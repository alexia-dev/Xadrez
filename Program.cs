using System;

namespace Xadrez;

public enum Cor { Branca, Preta }

public sealed class Peca
{
    public char Simbolo { get; }
    public Cor Cor { get; }
    public Peca(char simbolo, Cor cor) { Simbolo = simbolo; Cor = cor; }
}

public static class Program
{
    static readonly Peca?[,] Tabuleiro = new Peca?[8,8];
    static Cor turno = Cor.Branca;

    public static void Main()
    {
        Console.Title = "Xadrez • Jogo";
        Inicializar();
        while (true)
        {
            Limpar();
            Cabecalho();
            MostrarTabuleiro();

            Console.WriteLine();
            Console.WriteLine("  Digite uma jogada no formato E2 E4.");
            Console.WriteLine("  Comandos: R = reiniciar | S = sair");
            Console.Write($"\n  {(turno == Cor.Branca ? "♙ BRANCAS" : "♟ PRETAS")} > ");

            string entrada = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            if (entrada == "S") return;
            if (entrada == "R") { Inicializar(); continue; }

            if (!TentarMover(entrada, out string mensagem))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  ✕ {mensagem}");
                Console.ResetColor();
                Console.WriteLine("  Pressione qualquer tecla...");
                Console.ReadKey(true);
                continue;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ✓ {mensagem}");
            Console.ResetColor();
            Console.WriteLine("  Pressione qualquer tecla...");
            Console.ReadKey(true);
        }
    }

    static void Inicializar()
    {
        Array.Clear(Tabuleiro);
        turno = Cor.Branca;

        char[] maiores = { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' };
        for (int c = 0; c < 8; c++)
        {
            Tabuleiro[0,c] = new Peca(maiores[c], Cor.Preta);
            Tabuleiro[1,c] = new Peca('P', Cor.Preta);
            Tabuleiro[6,c] = new Peca('P', Cor.Branca);
            Tabuleiro[7,c] = new Peca(maiores[c], Cor.Branca);
        }
    }

    static bool TentarMover(string entrada, out string mensagem)
    {
        mensagem = "";
        var partes = entrada.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length != 2 || !Coordenada(partes[0], out int l1, out int c1) || !Coordenada(partes[1], out int l2, out int c2))
        {
            mensagem = "Formato inválido. Use, por exemplo: E2 E4.";
            return false;
        }

        Peca? peca = Tabuleiro[l1,c1];
        if (peca is null)
        {
            mensagem = "Não existe peça nessa casa.";
            return false;
        }

        if (peca.Cor != turno)
        {
            mensagem = "Essa peça pertence ao outro jogador.";
            return false;
        }

        Peca? destino = Tabuleiro[l2,c2];
        if (destino?.Cor == turno)
        {
            mensagem = "Você não pode capturar uma peça sua.";
            return false;
        }

        if (!MovimentoValido(peca, l1,c1,l2,c2))
        {
            mensagem = $"Movimento inválido para a peça {peca.Simbolo}.";
            return false;
        }

        string captura = destino is null ? "" : $" Capturou {destino.Simbolo}.";
        Tabuleiro[l2,c2] = peca;
        Tabuleiro[l1,c1] = null;
        turno = turno == Cor.Branca ? Cor.Preta : Cor.Branca;

        mensagem = $"Jogada realizada: {entrada}.{captura}";
        return true;
    }

    static bool MovimentoValido(Peca p, int l1, int c1, int l2, int c2)
    {
        int dl = l2 - l1;
        int dc = c2 - c1;
        int adl = Math.Abs(dl);
        int adc = Math.Abs(dc);

        return p.Simbolo switch
        {
            'P' => MovimentoPeao(p.Cor, l1,c1,l2,c2),
            'R' => adl <= 1 && adc <= 1 && (adl + adc > 0),
            'N' => (adl == 2 && adc == 1) || (adl == 1 && adc == 2),
            'B' => adl == adc && CaminhoLivre(l1,c1,l2,c2),
            'Q' => ((adl == adc) || dl == 0 || dc == 0) && CaminhoLivre(l1,c1,l2,c2),
            'K' => adl <= 1 && adc <= 1 && (adl + adc > 0),
            _ => false
        };
    }

    static bool MovimentoPeao(Cor cor, int l1, int c1, int l2, int c2)
    {
        int direcao = cor == Cor.Branca ? -1 : 1;
        int linhaInicial = cor == Cor.Branca ? 6 : 1;
        Peca? destino = Tabuleiro[l2,c2];

        if (c1 == c2 && destino is null && l2 - l1 == direcao) return true;
        if (c1 == c2 && destino is null && l1 == linhaInicial && l2 - l1 == 2 * direcao &&
            Tabuleiro[l1 + direcao,c1] is null) return true;

        return Math.Abs(c2 - c1) == 1 && l2 - l1 == direcao && destino is not null && destino.Cor != cor;
    }

    static bool CaminhoLivre(int l1, int c1, int l2, int c2)
    {
        int dl = Math.Sign(l2 - l1);
        int dc = Math.Sign(c2 - c1);
        int l = l1 + dl, c = c1 + dc;

        while (l != l2 || c != c2)
        {
            if (Tabuleiro[l,c] is not null) return false;
            l += dl; c += dc;
        }
        return true;
    }

    static bool Coordenada(string texto, out int linha, out int coluna)
    {
        linha = coluna = -1;
        if (texto.Length != 2) return false;
        char c = texto[0];
        char l = texto[1];
        if (c < 'A' || c > 'H' || l < '1' || l > '8') return false;
        coluna = c - 'A';
        linha = 8 - (l - '0');
        return true;
    }

    static void MostrarTabuleiro()
    {
        Console.WriteLine("       A   B   C   D   E   F   G   H");
        Console.WriteLine("     ┌───┬───┬───┬───┬───┬───┬───┬───┐");
        for (int l = 0; l < 8; l++)
        {
            Console.Write($"  {8-l}  │");
            for (int c = 0; c < 8; c++)
            {
                Peca? p = Tabuleiro[l,c];
                Console.Write(p is null ? "   │" : $" {Simbolo(p)} │");
            }
            Console.WriteLine($" {8-l}");
            if (l < 7) Console.WriteLine("     ├───┼───┼───┼───┼───┼───┼───┼───┤");
        }
        Console.WriteLine("     └───┴───┴───┴───┴───┴───┴───┴───┘");
        Console.WriteLine("       A   B   C   D   E   F   G   H");
    }

    static string Simbolo(Peca p) => p.Cor == Cor.Branca
        ? $"\u001b[1;97m{PecaBranca(p.Simbolo)}\u001b[0m"
        : $"\u001b[1;30m{PecaPreta(p.Simbolo)}\u001b[0m";

    static char PecaBranca(char p) => p switch { 'K'=>'♔','Q'=>'♕','R'=>'♖','B'=>'♗','N'=>'♘','P'=>'♙',_=>p };
    static char PecaPreta(char p) => p switch { 'K'=>'♚','Q'=>'♛','R'=>'♜','B'=>'♝','N'=>'♞','P'=>'♟',_=>p };

    static void Cabecalho()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ♟ XADREZ");
        Console.ResetColor();
        Console.WriteLine("  ─────────────────────────────────────────────");
        Console.WriteLine($"  Turno: {(turno == Cor.Branca ? "BRANCAS ♙" : "PRETAS ♟")}");
    }

    static void Limpar() => Console.Clear();
}
