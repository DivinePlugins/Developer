namespace Divine.Plugin.Engine.Processor.SendTables;

using System;
using System.Collections.Generic;

using Divine.Plugin.Engine.IO.Source2;
using Divine.Plugin.Engine.IO.Source2.Fields;
using Divine.Plugin.Engine.Model;
using Divine.Protobufs.Dota2;
using Divine.Common.Log;

internal sealed class FieldGenerator
{
    private static readonly Dictionary<BuildNumberRange, Action<SerializerId, FieldData>> PATCHES = new();

    private static readonly SerializerId SID_PITCH_YAW = new SerializerId("CBodyComponentBaseAnimatingOverlay", 3);

    private readonly CSVCMsg_FlattenedSerializer protoMessage;

    private readonly FieldData[] fieldData;

    private readonly HashSet<int> checkedNames;

    private readonly List<Action<SerializerId, FieldData>> patchFuncs;

    private readonly Dictionary<SerializerId, Serializer> serializers = new();

    static FieldGenerator()
    {
        PATCHES[new BuildNumberRange(null, 954)] = (serializerId, field) =>
        {
            switch (field.name)
            {
                case "m_flMana":
                case "m_flMaxMana":
                    ProtoDecoderProperties up = field.decoderProperties;
                    if (up.highValue == 3.4028235E38f)
                    {
                        up.lowValue = null;
                        up.highValue = 8192.0f;
                    }
                    break;
            }
        };

        PATCHES[new BuildNumberRange(null, 990)] = (serializerId, field) =>
        {
            switch (field.name)
            {
                case "dirPrimary":
                case "localSound":
                case "m_attachmentPointBoneSpace":
                case "m_attachmentPointRagdollSpace":
                case "m_flElasticity":
                case "m_location":
                case "m_poolOrigin":
                case "m_ragPos":
                case "m_vecEndPos":
                case "m_vecEyeExitEndpoint":
                case "m_vecGunCrosshair":
                case "m_vecLadderDir":
                case "m_vecPlayerMountPositionBottom":
                case "m_vecPlayerMountPositionTop":
                case "m_viewtarget":
                case "m_WorldMaxs":
                case "m_WorldMins":
                case "origin":
                case "vecExtraLocalOrigin":
                case "vecLocalOrigin":
                    field.decoderProperties.encoderType = "coord";
                    break;

                case "angExtraLocalAngles":
                case "angLocalAngles":
                case "m_angInitialAngles":
                case "m_ragAngles":
                case "m_vLightDirection":
                    field.decoderProperties.encoderType = "QAngle";
                    break;

                case "m_vecLadderNormal":
                    field.decoderProperties.encoderType = "normal";
                    break;

                case "m_angRotation":
                    field.decoderProperties.encoderType = SID_PITCH_YAW.Equals(serializerId) ? "qangle_pitch_yaw" : "QAngle";
                    break;
            }
        };

        PATCHES[new BuildNumberRange(1016, 1026)] = (serializerId, field) =>
        {
            switch (field.name)
            {
                case "m_bWorldTreeState":
                case "m_ulTeamLogo":
                case "m_ulTeamBaseLogo":
                case "m_ulTeamBannerLogo":
                case "m_iPlayerIDsInControl":
                case "m_bItemWhiteList":
                case "m_iPlayerSteamID":
                    field.decoderProperties.encoderType = "fixed64";
                    break;
            }
        };

        PATCHES[new BuildNumberRange(null, null)] = (serializerId, field) =>
        {
            switch (field.name)
            {
                case "m_flSimulationTime":
                case "m_flAnimTime":
                    field.decoderProperties.encoderType = "simulationtime";
                    break;
            }
        };

        PATCHES[new BuildNumberRange(null, null)] = (serializerId, field) =>
        {
            switch (field.name)
            {
                case "m_flRuneTime":
                    ProtoDecoderProperties up = field.decoderProperties;
                    if (up.highValue == float.MaxValue && up.lowValue == -float.MaxValue)
                    {
                        up.lowValue = null;
                        up.highValue = null;
                    }
                    break;
            }
        };
    }

    public FieldGenerator(CSVCMsg_FlattenedSerializer protoMessage, int buildNumber)
    {
        this.protoMessage = protoMessage;
        this.fieldData = new FieldData[protoMessage.Fields.Count];
        this.checkedNames = new();
        this.patchFuncs = new();

        foreach (var patchEntry in PATCHES)
        {
            if (patchEntry.Key.appliesTo(buildNumber))
            {
                patchFuncs.Add(patchEntry.Value);
            }
        }
    }

    public void createFields()
    {
        for (int i = 0; i < fieldData.Length; i++)
        {
            fieldData[i] = generateFieldData(protoMessage.Fields[i]);
        }
        for (int i = 0; i < protoMessage.Serializers.Count; i++)
        {
            Serializer serializer = generateSerializer(protoMessage.Serializers[i]);
            serializers[serializer.getId()] = serializer;
        }
    }

    public DTClass createDTClass(string name)
    {
        SerializerField field = new SerializerField(FieldType.forString(name), serializers[new SerializerId(name, 0)]);
        return new DTClass(field);
    }

    private FieldData generateFieldData(ProtoFlattenedSerializerField_t proto)
    {
        return new FieldData(
                FieldType.forString(sym(proto.VarTypeSym)),
                fieldNameFunction(proto),
                new ProtoDecoderProperties(
                        proto.HasEncodeFlags ? proto.EncodeFlags : null,
                        proto.HasBitCount ? proto.BitCount : null,
                        proto.HasLowValue ? proto.LowValue : null,
                        proto.HasHighValue ? proto.HighValue : null,
                        proto.HasVarEncoderSym ? sym(proto.VarEncoderSym) : null),
                proto.HasFieldSerializerNameSym ? new SerializerId(sym(proto.FieldSerializerNameSym), proto.FieldSerializerVersion) : null
        );
    }

    private Serializer generateSerializer(ProtoFlattenedSerializer_t proto)
    {
        SerializerId sid = new SerializerId(sym(proto.SerializerNameSym), proto.SerializerVersion);
        Field[] fields = new Field[proto.FieldsIndex.Count];
        string[] fieldNames = new string[proto.FieldsIndex.Count];
        for (int i = 0; i < fields.Length; i++)
        {
            int fi = proto.FieldsIndex[i];
            if (fieldData[fi].field == null)
            {
                fieldData[fi].field = createField(sid, fieldData[fi]);
            }

            fields[i] = fieldData[fi].field;
            fieldNames[i] = fieldData[fi].name;
        }

        return new Serializer(sid, fields, fieldNames);
    }

    private Field createField(SerializerId sId, FieldData fd)
    {
        foreach (var patchFunc in patchFuncs)
        {
            patchFunc(sId, fd);
        }

        FieldType elementType;
        switch (fd.category)
        {
            case FieldCategory.ARRAY:
                elementType = fd.fieldType.getElementType();
                break;
            case FieldCategory.VECTOR:
                elementType = fd.fieldType.getGenericType();
                break;
            default:
                elementType = fd.fieldType;
                break;
        }

        Field elementField;
        if (fd.serializerId != null)
        {
            if (fd.category == FieldCategory.POINTER)
            {
                elementField = new PointerField(elementType, serializers[fd.serializerId]);
            }
            else
            {
                elementField = new SerializerField(elementType, serializers[fd.serializerId]);
            }
        }
        else
        {
            elementField = new ValueField(elementType, DecoderFactory.createDecoder(fd.decoderProperties, elementType.getBaseType()));
        }

        switch (fd.category)
        {
            case FieldCategory.ARRAY:
                return new ArrayField(fd.fieldType, elementField, fd.getArrayElementCount());
            case FieldCategory.VECTOR:
                return new VectorField(fd.fieldType, elementField);
            default:
                return elementField;
        }
    }

    private string sym(int i)
    {
        return protoMessage.Symbols[i];
    }

    private string fieldNameFunction(ProtoFlattenedSerializerField_t field)
    {
        int nameSym = field.VarNameSym;
        var name = sym(nameSym);
        if (!checkedNames.Contains(nameSym))
        {
            if (name.IndexOf('.') != -1)
            {
                LogManager.Warn($"replay contains field with invalid name '{name}'. Please open a github issue!");
            }

            checkedNames.Add(nameSym);
        }

        return name;
    }

    private sealed class FieldData
    {
        public readonly FieldType fieldType;
        public readonly string name;
        public readonly ProtoDecoderProperties decoderProperties;
        public readonly SerializerId? serializerId;

        public readonly FieldCategory category;
        public Field field;

        public FieldData(FieldType fieldType, string name, ProtoDecoderProperties decoderProperties, SerializerId? serializerId)
        {
            this.fieldType = fieldType;
            this.name = name;
            this.decoderProperties = decoderProperties;
            this.serializerId = serializerId;

            if (determineIsPointer())
            {
                category = FieldCategory.POINTER;
            }
            else if (determineIsVector())
            {
                category = FieldCategory.VECTOR;
            }
            else if (determineIsArray())
            {
                category = FieldCategory.ARRAY;
            }
            else
            {
                category = FieldCategory.VALUE;
            }
        }

        private bool determineIsPointer()
        {
            if (fieldType.isPointer())
                return true;
            switch (fieldType.getBaseType())
            {
                case "CBodyComponent":
                case "CLightComponent":
                case "CPhysicsComponent":
                case "CRenderComponent":
                case "CPlayerLocalData":
                    return true;
            }
            return false;
        }

        private bool determineIsVector()
        {
            if (serializerId != null)
                return true;
            switch (fieldType.getBaseType())
            {
                case "CUtlVector":
                case "CNetworkUtlVectorBase":
                    return true;
                default:
                    return false;
            }
        }

        private bool determineIsArray()
        {
            return fieldType.getElementCount() != null && !"char".Equals(fieldType.getBaseType());
        }

        public int getArrayElementCount()
        {
            var elementCount = fieldType.getElementCount();
            switch (elementCount)
            {
                case "MAX_ITEM_STOCKS":
                    return 8;
                case "MAX_ABILITY_DRAFT_ABILITIES":
                    return 48;
                default:
                    return int.Parse(elementCount);
            }
        }

    }

    private enum FieldCategory
    {
        POINTER,
        VECTOR,
        ARRAY,
        VALUE
    }
}