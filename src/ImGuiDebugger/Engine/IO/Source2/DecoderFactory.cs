namespace Divine.Plugin.Engine.IO.Source2;

using System.Collections.Generic;

using Divine.Plugin.Engine.IO.Decoder;
using Divine.Plugin.Engine.IO.Decoder.Factory;
using Divine.Common.Log;

internal static class DecoderFactory
{
    private static readonly IDecoder DEFAULT_DECODER = new IntVarUnsignedDecoder();

    private static readonly Dictionary<string, IDecoderFactory> FACTORIES = new();

    private static readonly Dictionary<string, IDecoder> DECODERS = new();

    static DecoderFactory()
    {
        // Unsigned ints
        FACTORIES["uint64"] = new LongUnsignedDecoderFactory();

        // Floats
        FACTORIES["float32"] = new FloatDecoderFactory();
        FACTORIES["CNetworkedQuantizedFloat"] = new FloatDecoderFactory();
        FACTORIES["QAngle"] = new QAngleDecoderFactory();

        // Specials
        FACTORIES["Vector2D"] = new VectorDecoderFactory(2);
        FACTORIES["Vector"] = new VectorDecoderFactory(3);
        FACTORIES["Vector4D"] = new VectorDecoderFactory(4);
        FACTORIES["Quaternion"] = new VectorDecoderFactory(4);

        // Booleans
        DECODERS["bool"] = new BoolDecoder();

        // Unsigned ints
        DECODERS["uint8"] = new IntVarUnsignedDecoder();
        DECODERS["uint16"] = new IntVarUnsignedDecoder();
        DECODERS["uint32"] = new IntVarUnsignedDecoder();

        // Signed ints
        DECODERS["int8"] = new IntVarSignedDecoder();
        DECODERS["int16"] = new IntVarSignedDecoder();
        DECODERS["int32"] = new IntVarSignedDecoder();
        DECODERS["int64"] = new LongVarSignedDecoder();

        // Strings
        DECODERS["CUtlSymbolLarge"] = new StringZeroTerminatedDecoder();
        DECODERS["char"] = new StringZeroTerminatedDecoder();
        DECODERS["CUtlString"] = new StringZeroTerminatedDecoder();

        DECODERS["CUtlStringToken"] = new IntVarUnsignedDecoder();

        // Handles
        DECODERS["CHandle"] = new IntVarUnsignedDecoder();
        DECODERS["CEntityHandle"] = new IntVarUnsignedDecoder();
        DECODERS["CGameSceneNodeHandle"] = new IntVarUnsignedDecoder();
        DECODERS["CBaseVRHandAttachmentHandle"] = new IntVarUnsignedDecoder();
        DECODERS["CStrongHandle"] = new LongVarUnsignedDecoder();

        // Colors
        DECODERS["Color"] = new IntVarUnsignedDecoder();
        DECODERS["color32"] = new IntVarUnsignedDecoder();

        // Specials
        DECODERS["HSequence"] = new IntMinusOneDecoder();
        DECODERS["GameTime_t"] = new FloatNoScaleDecoder();

        // Specials MY
        DECODERS["GameTick_t"] = new IntVarSignedDecoder(); // int32
        DECODERS["AbilityID_t"] = new IntVarSignedDecoder(); // int32
        DECODERS["PlayerID_t"] = new IntVarSignedDecoder(); // int32
        DECODERS["GuildID_t"] = new IntVarUnsignedDecoder(); // uint32
        DECODERS["PeriodicResourceID_t"] = new IntVarUnsignedDecoder(); // uint32
        DECODERS["item_definition_index_t"] = new IntVarUnsignedDecoder(); // uint32
        DECODERS["CavernCrawlMapVariant_t"] = new IntVarUnsignedDecoder(); // uint8
        DECODERS["CPlayerSlot"] = new IntVarUnsignedDecoder(); // uint32
        DECODERS["attrib_definition_index_t"] = new IntVarUnsignedDecoder(); // uint16
        DECODERS["itemid_t"] = new LongVarUnsignedDecoder(); // uint64
        DECODERS["style_index_t"] = new IntVarUnsignedDecoder(); // uint8
        DECODERS["CEntityIndex"] = new IntVarSignedDecoder(); // int32
        DECODERS["MatchID_t"] = new LongVarUnsignedDecoder(); // uint64
        DECODERS["LeagueID_t"] = new IntVarUnsignedDecoder(); // uint32
        DECODERS["AttachmentHandle_t"] = new IntVarUnsignedDecoder(); // uint8
        DECODERS["m_nWorldGroupId"] = new IntVarUnsignedDecoder(); // uint32
    }

    public static DecoderHolder createDecoder(string type)
    {
        return createDecoder(IDecoderProperties.Default, type);
    }

    public static DecoderHolder createDecoder(IDecoderProperties decoderProperties, string type)
    {
        IDecoder decoder;
        if (FACTORIES.TryGetValue(type, out var decoderFactory))
        {
            decoder = decoderFactory.createDecoder(decoderProperties);
        }
        else if (!DECODERS.TryGetValue(type, out decoder!))
        {
            LogManager.Debug($"don't know how to create decoder for {type}, assuming int.");
            decoder = DEFAULT_DECODER;
        }

        return new DecoderHolder(decoderProperties, decoder);
    }
}