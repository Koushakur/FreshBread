using HarmonyLib;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Styles;
using System;
using System.Reflection;
using UnityEngine;
using static FreshBread.Patches.Patch_ContextDocumentationUi.Patch_ContextDocumentationUi_BuildInterface;
using FreshBread;


public class ScreenSegmentTableSearchable : ScreenSegmentTable {

    private static readonly FieldInfo _tableField = AccessTools.Field(typeof(ScreenSegmentTable), "_table");
    private IInterpretter Placeholder => (IInterpretter)AccessTools.Field(typeof(ScreenSegmentTable), "_placeholder").GetValue(this);
    private MethodInfo WidthOfColumnInPixels => AccessTools.Method(typeof(ScreenSegmentTable), "WidthOfColumnInPixels");

    private Func<bool>[,] _cellRelevance;

    public ScreenSegmentTableSearchable(ConsoleUiScreen screen, int columns, int maxRows)
        : base(screen, columns, maxRows) {
        _cellRelevance = new Func<bool>[columns, maxRows];
    }

    public void SetCellRelevance(int col, int row, Func<bool> relevance) {
        if (col < _cellRelevance.GetLength(0) && row < _cellRelevance.GetLength(1))
            _cellRelevance[col, row] = relevance;
    }

    public IInterpretter GetCell(int col, int row) {
        var grid = (IInterpretter[,])_tableField.GetValue(this);
        return grid[col, row];
    }

    private bool CellIsRelevant(int col, int row) {

        if (_cellRelevance[col, row] != null && !_cellRelevance[col, row]())
            return false;

        return true;
    }

    public void ApplySearchRelevanceToCells(SearchBox searchBox) {

        for (int row = 0; row < Height; row++) {
            for (int col = 0; col < Width; col++) {
                if (GetCell(col, row) is StringDisplay sd) {
                    string plainText = FreshBreadGlobal.StripRichText(sd.GetDisplayString());
                    SetCellRelevance(col, row, () =>
                        string.IsNullOrEmpty(searchBox.Value) ||
                        plainText.Contains(searchBox.Value, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
    }

    protected override void UiEntry(ScreenSegmentDisplayOptions _s) {
        var table = (IInterpretter[,])_tableField.GetValue(this);
        int width = table.GetLength(0);  //Columns
        int height = table.GetLength(1); //Rows

        if (eTableOrder == TableOrder.Rows)
            DrawRows(table, width, height, _s);
        else
            DrawColumns(table, width, height, _s);
    }

    private void DrawRows(IInterpretter[,] table, int width, int height, ScreenSegmentDisplayOptions _s) {
        GUILayout.BeginVertical(new GUIContent(NameWhereApplicable), BackgroundStyleWhereApplicable);
        for (int row = 0; row < height; row++) {

            GUILayout.BeginHorizontal();
            for (int col = 0; col < width; col++) {
                IInterpretter interpretter = table[col, row];
                if (interpretter == null) continue;

                if (interpretter.ShouldIDisplay() && CellIsRelevant(col, row)) {
                    if (!SqueezeTable)
                        interpretter.PrescribedWidth = new PixelSizing(
                            (float)WidthOfColumnInPixels.Invoke(this, new object[] { col, _s.MasterRect.width }), Dimension.Width);
                    interpretter.Draw(_s._s);
                } else if (UseEmptyPlaceholders) {
                    Placeholder.Draw(_s._s);
                }
            }
            GUILayout.EndHorizontal();

            if (table[0, row] != null && RowDrawingGap > 0f)
                GUILayout.Space(RowDrawingGap);
        }
        GUILayout.EndVertical();
    }

    private void DrawColumns(IInterpretter[,] table, int width, int height, ScreenSegmentDisplayOptions _s) {
        GUILayout.BeginHorizontal(new GUIContent(NameWhereApplicable), BackgroundStyleWhereApplicable);
        if (SqueezeTable) GUILayout.FlexibleSpace();

        for (int col = 0; col < width; col++) {
            if (!SqueezeTable)
                GUILayout.BeginVertical(GUILayout.Width((float)WidthOfColumnInPixels.Invoke(this, new object[] { col, _s.MasterRect.width })));
            else
                GUILayout.BeginVertical();

            if (ColumnHeadings[col] != null)
                GUILayout.Label(ColumnHeadings[col], ConsoleStyles.Instance.Styles.Segments.ColumnHeading.Style);

            for (int row = 0; row < height; row++) {
                IInterpretter interpretter = table[col, row];
                if (interpretter == null) continue;

                if (interpretter.ShouldIDisplay() && CellIsRelevant(col, row))
                    interpretter.Draw(_s._s);
                else if (UseEmptyPlaceholders)
                    Placeholder.Draw(_s._s);
            }
            GUILayout.EndVertical();
        }

        if (SqueezeTable) GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
