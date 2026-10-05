using System;
using System.Drawing;
using System.Windows.Forms;

namespace XadrezGui;

public class MainForm : Form
{
    private readonly TableLayoutPanel board = new() { RowCount = 8, ColumnCount = 8, Dock = DockStyle.Left, Size = new Size(480, 480) };
    private readonly Label status = new() { Text = "Turno: Brancas — clique em uma casa", AutoSize = true, Location = new Point(500, 25) };

    public MainForm()
    {
        Text = "Xadrez - GUI básica";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(760, 520);
        for (int r = 0; r < 8; r++) board.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5f));
        for (int c = 0; c < 8; c++) board.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5f));
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                var b = new Button { Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Symbol", 22) };
                if ((r + c) % 2 == 0) b.BackColor = Color.Beige;
                else b.BackColor = Color.Sienna;
                b.Text = r == 1 ? "♟" : r == 6 ? "♙" : r == 0 && (c == 0 || c == 7) ? "♜" : r == 7 && (c == 0 || c == 7) ? "♖" : "";
                int rr = r, cc = c;
                b.Click += (_, _) => status.Text = $"Casa selecionada: {(char)('a' + cc)}{8 - rr}";
                board.Controls.Add(b, c, r);
            }
        Controls.Add(board);
        Controls.Add(status);
    }
}

public static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
