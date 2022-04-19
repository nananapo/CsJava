namespace CsJava;

public class LineNumberAttribute : AttributeInfo
{
    public readonly ushort line_number_table_length;
    public readonly List<(ushort start_pc, ushort line_number)> line_number_table;

    public LineNumberAttribute(uint attributeLength, ushort line_number_table_length, List<(ushort,ushort)> line_number_table) : base(attributeLength)
    {
        this.line_number_table_length = line_number_table_length;
        this.line_number_table = line_number_table;
    }
}