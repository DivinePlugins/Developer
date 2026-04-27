namespace Divine.Plugin.Engine;

using Divine.Plugin.Engine.Model.Source2;
using Divine.Plugin.Engine.Model.State;

internal interface IDTClass
{
    string getDtName();

    int getClassId();
    void setClassId(int classId);

    IEntityState getEmptyState();

    string? getNameForFieldPath(IFieldPath fp);

    IFieldPath? getFieldPathForName(string property);
}