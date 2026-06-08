namespace UndirLaFlota.Juego;

public class Tablero
{
    #region Variables
    public List<List<int>> TableroList { get; set; }
    public bool IsPlacingBoats { get; set; } = true;
    private List<int> Barcos;
    private int aciertos = 0;
    private int TotalPuntos;
    public int Dim;
    public List<int> boatToPlace => new List<int> { 2, 3, 3, 5, 6 };
    public List<Tuple<int, int>> LastShipSunk { get; private set; } = new List<Tuple<int, int>>();
    #endregion

    #region Constructor de la clase Tablero
    /// <summary>
    /// Método que gestiona la lógica para añadir los barcos en el tablero.
    /// </summary>
    /// <param name="iaBoard">Booleano para indicar si se deben generan o no barcos en el tablero creado.</param>
    public Tablero(bool iaBoard)
    {
        Dim = 10;
        TableroList = new List<List<int>>();
        Barcos= new List<int>();
        Barcos.Add(6);
        Barcos.Add(5);
        Barcos.Add(3);
        Barcos.Add(3);
        Barcos.Add(2);

        foreach (int i in Barcos)
            TotalPuntos += i;

        TableroLimpio();

        if (iaBoard)
        {
            GenerarTablero();
            IsPlacingBoats = false;
        }
    }
    #endregion

    #region Métodos Propios

    /// <summary>
    /// Método para reiniciar el tablero de nuevo.
    /// </summary>
    private void TableroLimpio()
    {
        aciertos = 0;

        for (int i = 0; i < Dim; i++)
        {
            TableroList.Add(new List<int>());

            for (int j = 0; j < Dim; j++)
                TableroList[i].Add(0);
        }
    }

    /// <summary>
    /// Método para generar el tablero y añadir los barcos (de manera automática).
    /// </summary>
    public void GenerarTablero()
    {
        foreach (int i in Barcos)
            GenerarBarco(i);
    }

    /// <summary>
    /// Método que gestiona la lógica de implementar los barcos de la IA teniendo en cuenta su tamaño.
    /// </summary>
    /// <param name="tamano"></param>
    private void GenerarBarco(int tamano)
    {
        Random random = new Random();
        int x = 0;
        int y = 0;
        int direction = 0;
        bool entra = false;

        while (!entra)
        {
            entra = true;
            x = random.Next(0, Dim);
            y = random.Next(0, Dim);
            direction = random.Next(0, 4);

            int P_x = x;
            int P_y = y;
            int pos = 0;

            while (pos < tamano)
            {
                // Se comprueba el tamaño del tablero para que no se salga.
                if (P_x < 0 || P_x >= Dim || P_y < 0 || P_y >= Dim)
                {
                    entra = false;
                    break;
                }

                // Se comprueba que no haya barcos al lado colocados.
                if (!IsSafeZone(P_x, P_y))
                {
                    entra = false;
                    break;
                }

                // Se avanza en función de la dirección.
                switch (direction)
                {
                    case 0: // Abajo
                        P_x++;
                        break;
                    case 1: // Arriba
                        P_x--;
                        break;
                    case 2: // Derecha
                        P_y++;
                        break;
                    case 3: // Izquierda
                        P_y--;
                        break;
                }

                pos++;
            }

            // Cuando el barco está posicionado correctamente, se añade definitivamente al tablero.
            if (entra)
            {
                P_x = x;
                P_y = y;
                pos = 0;

                while (pos < tamano)
                {
                    TableroList[P_x][P_y] = 2;

                    switch (direction)
                    {
                        case 0: // Abajo
                            P_x++;
                            break;
                        case 1: // Arriba
                            P_x--;
                            break;
                        case 2: // Derecha
                            P_y++;
                            break;
                        case 3: // Izquierda
                            P_y--;
                            break;
                    }

                    pos++;
                }
            }
        }
    }

    /// <summary>
    /// Método que comprueba que las casillas de alrededor no hay más barcos.
    /// </summary>
    private bool IsSafeZone(int fila, int col)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int nuevaFila = fila + i;
                int nuevaCol = col + j;

                // Se comprueba si la casilla vecina está dentro de los límites del tablero.
                if (nuevaFila >= 0 && nuevaFila < Dim && nuevaCol >= 0 && nuevaCol < Dim)
                {
                    // Si se encuentra un barco en alguna de las casillas colindantes, devuelve false. No se puede colocar.
                    if (TableroList[nuevaFila][nuevaCol] == 2) return false;
                }
            }
        }
        return true; // Si cumple los requisitos anteriores, se devuelve true para permitir colocar el barco.
    }

    /// <summary>
    /// Método que gestiona las jugadas en el tablero.
    /// Indica si el jugador ha hecho click en una casilla de agua, o ha tocado un barco.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public String Jugada(int x, int y)
    {
        if (TableroList[x][y] == 0)
        {
            TableroList[x][y] = 1;
            return "Agua";
        }

        else if (TableroList[x][y] == 1 || TableroList[x][y] == 3) return null;

        else if (TableroList[x][y] == 2)
        {
            aciertos++;
            TableroList[x][y] = 3;

            if (aciertos >= TotalPuntos)
                return "Partida finalizada";

            if (CheckSunkBoat(x, y))
                return "Hundido";

            return "Tocado";
        }

        return null;
    }

    /// <summary>
    /// Método que cuenta cuántos barcos sin tocar quedan en el tablero.
    /// </summary>
    /// <returns></returns>
    public int CountBoats()
    {
        int totalLiveBoats = 0;

        for (int i = 0; i < Dim; i++)
            for (int j = 0; j < Dim; j++)
                if (TableroList[i][j] == 2) totalLiveBoats++;
        return totalLiveBoats;
    }

    /// <summary>
    /// Escanea el tablero actual y comprueba si los barcos dibujados son válidos.
    /// </summary>
    public bool CheckBoatsPositions(out List<int> boatsPlaced)
    {
        // Se clona el tablero para poder comprobar las posiciones de los barcos sin afectar al tablero real.
        var copiaTablero = TableroList.Select(fila => fila.ToList()).ToList();
        boatsPlaced = new List<int>();

        for (int i = 0; i < Dim; i++)
        {
            for (int j = 0; j < Dim; j++)
            {
                if (copiaTablero[i][j] == 2)
                {
                    int tamano = AnalyzeBoatInBoard(copiaTablero, i, j);

                    if (tamano > 0)
                        boatsPlaced.Add(tamano); // Se añade a la lista de barcos colocados.
                    else if (tamano == -1) // Devuelve false si detecta que el barco no tiene el tamaño correcto o está colocado de manera no permitida.
                        return false;
                }
            }
        }

        boatsPlaced.Sort(); // Se ordenan de menor a mayor la lista con los tamaños de los barcos.
        var requeridosOrdenados = new List<int>(boatToPlace); // Se crea una lista de los barcos que se pueden poner (sus tamaños ya están establecidos desde el inicio).
        requeridosOrdenados.Sort(); // Se ordena de menor a mayor la lista de los barcos establecidos.

        return boatsPlaced.SequenceEqual(requeridosOrdenados); //Se comparan las secuencias de ambas listas. Si coinciden, devuelve true, si no coinciden, devuelve false.
    }

    /// <summary>
    /// Método auxiliar para comprobar los barcos encontrados en la copia del tablero.
    /// </summary>
    /// <param name="copia"></param>
    /// <param name="f"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    private int AnalyzeBoatInBoard(List<List<int>> copia, int f, int c)
    {
        var boatBoxes = new List<Tuple<int, int>>();

        ConnectedBoatBoxes(copia, f, c, boatBoxes);

        if (boatBoxes.Count == 0) return 0;
        if (boatBoxes.Count == 1) return 1;

        // Se obtienen los exptremos de los barcos.
        int minRow = boatBoxes.Min(x => x.Item1);
        int maxRow = boatBoxes.Max(x => x.Item1);
        int minCol = boatBoxes.Min(x => x.Item2);
        int maxCol = boatBoxes.Max(x => x.Item2);

        bool isHorizontal = (minRow == maxRow); // Si la fila min y max coinciden es que él barco está en horizontal.
        bool isVertical = (minCol == maxCol); // Si la columna min y max coinciden es que él barco está en vertical.

        if (!isHorizontal && !isVertical) return -1; // Si no coincide ninguna dirección es que está posicionado de manera no permitida.

        return boatBoxes.Count; // Total de casillas.
    }

    /// <summary>
    /// Método auxiliar para guardar los barcos de manera independiente.
    /// </summary>
    /// <param name="copia"></param>
    /// <param name="f"></param>
    /// <param name="c"></param>
    /// <param name="coordBoat"></param>
    private void ConnectedBoatBoxes(List<List<int>> copia, int f, int c, List<Tuple<int, int>> coordBoat)
    {
        if (f < 0 || f >= Dim || c < 0 || c >= Dim) return;

        if (copia[f][c] != 2) return;

        // Se almacenan las coordenadas del barco.
        coordBoat.Add(new Tuple<int, int>(f, c));
        copia[f][c] = 0;

        // Se buscan vecinos en sus casillas colindantes.
        ConnectedBoatBoxes(copia, f + 1, c, coordBoat);
        ConnectedBoatBoxes(copia, f - 1, c, coordBoat);
        ConnectedBoatBoxes(copia, f, c + 1, coordBoat);
        ConnectedBoatBoxes(copia, f, c - 1, coordBoat);
    }

    /// <summary>
    /// Método que comprueba si el barco está solo tocado o hundido.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CheckSunkBoat(int x, int y)
    {
        LastShipSunk.Clear();
        var revisados = new HashSet<Tuple<int, int>>();

        if (!BoatOnlyTouched(x, y, revisados))
        {
            LastShipSunk = revisados.ToList();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Método para comprobar si el barco tiene partes sin tocar.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="boat"></param>
    /// <returns></returns>
    private bool BoatOnlyTouched(int x, int y, HashSet<Tuple<int, int>> boat)
    {
        if (x < 0 || x >= Dim || y < 0 || y >= Dim) return false;

        if (TableroList[x][y] == 0 || TableroList[x][y] == 1) return false;

        var pos = new Tuple<int, int>(x, y);
        if (boat.Contains(pos)) return false;

        boat.Add(pos);

        if (TableroList[x][y] == 2) return true;

        // Se hace recursividad para comprobar que las casillas de alrededor han sido tocadas o no.
        if (TableroList[x][y] == 3)
        {
            // Si cualquiera de las casillas colindantes no ha sido tocada, el método devuelve True, avisando de que todavía quedan partes del barco sin ser tocadas.
            return BoatOnlyTouched(x + 1, y, boat) || // Mirar abajo
                   BoatOnlyTouched(x - 1, y, boat) || // Mirar arriba
                   BoatOnlyTouched(x, y + 1, boat) || // Mirar derecha
                   BoatOnlyTouched(x, y - 1, boat);   // Mirar izquierda
        }

        return false;
    }

    /// <summary>
    /// Método secundario para ver dónde ha colocado la IA sus barcos.
    /// (Lo he creado para ir más rápidos en las pruebas. TODO: Comentar este método luego).
    /// </summary>
    public void ImprimirTablero(string nombreBoard)
    {
        System.Diagnostics.Trace.WriteLine($"\n=== ESTADO DEL TABLERO: {nombreBoard.ToUpper()} ===");

        // Imprime los números de las columnas
        System.Diagnostics.Trace.Write("   ");
        for (int c = 0; c < Dim; c++)
        {
            System.Diagnostics.Trace.Write(c + " ");
        }
        System.Diagnostics.Trace.WriteLine("\n-----------------------");

        // Recorre las filas
        for (int i = 0; i < Dim; i++)
        {
            System.Diagnostics.Trace.Write(i + " | ");

            for (int j = 0; j < Dim; j++)
            {
                int casilla = TableroList[i][j];

                if (casilla == 2)
                    System.Diagnostics.Trace.Write("B ");
                else if (casilla == 3)
                    System.Diagnostics.Trace.Write("X ");
                else if (casilla == 1)
                    System.Diagnostics.Trace.Write("~ ");
                else
                    System.Diagnostics.Trace.Write(". ");
            }
            System.Diagnostics.Trace.WriteLine("");
        }
        System.Diagnostics.Trace.WriteLine("==========================\n");
    }
    #endregion
}