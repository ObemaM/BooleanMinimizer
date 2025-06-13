public class KarnaughGroup
{
    public List<(int Row, int Column)> Positions { get; set; }
    public string Description { get; set; }

    public KarnaughGroup(List<(int Row, int Column)> positions, string description)
    {
        Positions = positions;
        Description = description;
    }
}