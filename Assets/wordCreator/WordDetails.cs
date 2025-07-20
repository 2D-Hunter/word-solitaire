using System.Collections.Generic;

[System.Serializable]
public class GridSize
{
    public int rows;
    public int cols;
}

[System.Serializable]
public class WordLevelCell
{
    public int row;
    public int col;
    public List<string> letters;
}

[System.Serializable]
public class WordLevelLayout
{
    public int level;
    public List<WordLevelCell> layout;
}

[System.Serializable]
public class WordLevelSet
{
    public GridSize gridSize;
    public List<WordLevelLayout> levels;
}
