namespace Blazor_Backgammon.ViewModels;

public class BackgammonViewModel
{
    public int TotalFieldSpaces => 24;
    public int HalfFieldSpaces => TotalFieldSpaces / 2;
    public List<List<int>> GameField { get; set; } = new();

    public BackgammonViewModel()
    {
        SetupGame();
    }

    public void SetupGame()
    {
        GameField.Clear();
        GameField.Add(new List<int>() { 1,1 });
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>() { 2, 2, 2, 2, 2});
        GameField.Add(new List<int>());
        GameField.Add(new List<int>() { 2, 2, 2});
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>() { 1, 1, 1, 1, 1});
        GameField.Add(new List<int>() { 2, 2, 2, 2, 2});
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>() { 1, 1, 1 });
        GameField.Add(new List<int>());
        GameField.Add(new List<int>() { 1, 1, 1, 1, 1});
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>());
        GameField.Add(new List<int>() { 2, 2 });
    }
}
