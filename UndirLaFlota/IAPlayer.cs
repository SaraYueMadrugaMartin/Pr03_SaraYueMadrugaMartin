using UndirLaFlota.Juego;

namespace UndirLaFlota
{
    class IAPlayer
    {
        #region Variables
        private bool iaAttack = false;
        private int firstRowTouched = -1;
        private int fisrtColTouched = -1;
        private int lastRowTouched = -1;
        private int lastColTouched = -1;

        private readonly List<Tuple<int, int>> nextDirections = new List<Tuple<int, int>>
        {
            new Tuple<int, int>(-1, 0),
            new Tuple<int, int>(1, 0),
            new Tuple<int, int>(0, -1),
            new Tuple<int, int>(0, 1)
        };
        #endregion

        #region Métodos Propios
        /// <summary>
        /// Método que gestiona la coordenada donde va a disparar la IA.
        /// Si no tiene ningún barco detectado, dispara de manera aleatoria.
        /// Si tiene un barco detectado, intenta seguir las casillas colindantes hasta hundirlo entero.
        /// </summary>
        /// <param name="boardPlayer01"></param>
        /// <returns></returns>
        public Tuple<int, int> IAShoot(Tablero boardPlayer01)
        {
            int row = -1;
            int col = -1;
            bool coordFound = false;

            if (iaAttack)
            {
                int intentosPivot = 0;

                while (!coordFound && intentosPivot < 2)
                {
                    List<Tuple<int, int>> tryDirections = new List<Tuple<int, int>>();

                    if (firstRowTouched != lastRowTouched || fisrtColTouched != lastColTouched)
                    {
                        if (firstRowTouched == lastRowTouched)
                        {
                            tryDirections.Add(new Tuple<int, int>(0, -1));
                            tryDirections.Add(new Tuple<int, int>(0, 1));
                        }
                        else if (fisrtColTouched == lastColTouched)
                        {
                            tryDirections.Add(new Tuple<int, int>(-1, 0));
                            tryDirections.Add(new Tuple<int, int>(1, 0));
                        }
                    }
                    else
                        tryDirections.AddRange(nextDirections);

                    Random rand = new Random();
                    var randomDirections = tryDirections.OrderBy(x => rand.Next()).ToList();

                    foreach (var dir in randomDirections)
                    {
                        int possibleRow = lastRowTouched + dir.Item1;
                        int possibleCol = lastColTouched + dir.Item2;

                        if (possibleRow >= 0 && possibleRow < boardPlayer01.Dim &&
                            possibleCol >= 0 && possibleCol < boardPlayer01.Dim)
                        {
                            int estado = boardPlayer01.TableroList[possibleRow][possibleCol];
                            if (estado == 0 || estado == 2)
                            {
                                row = possibleRow;
                                col = possibleCol;
                                coordFound = true;
                                break;
                            }
                        }
                    }

                    if (!coordFound && (firstRowTouched != lastRowTouched || fisrtColTouched != lastColTouched))
                    {
                        lastRowTouched = firstRowTouched;
                        lastColTouched = fisrtColTouched;
                        intentosPivot++;
                    }
                    else
                        break;
                }

                if (!coordFound)
                    iaAttack = false;
            }

            if (!coordFound)
            {
                Random random = new Random();
                int intentosCaza = 0;
                int maxIntentos = boardPlayer01.Dim * boardPlayer01.Dim * 2;

                while (!coordFound && intentosCaza < maxIntentos)
                {
                    intentosCaza++;
                    int f = random.Next(0, boardPlayer01.Dim);
                    int c = random.Next(0, boardPlayer01.Dim);
                    int boxState = boardPlayer01.TableroList[f][c];

                    if (boxState == 0 || boxState == 2)
                    {
                        row = f;
                        col = c;
                        coordFound = true;
                    }
                }
            }

            if (!coordFound)
                return new Tuple<int, int>(0, 0);

            return new Tuple<int, int>(row, col);
        }

        /// <summary>
        /// Método que guarda el resultado del último disparo que hace la IA
        /// </summary>
        /// <param name="result"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void SaveResult(string result, int row, int col)
        {
            if (result == "Tocado")
            {
                if (!iaAttack)
                {
                    iaAttack = true;
                    firstRowTouched = row;
                    fisrtColTouched = col;
                }

                lastRowTouched = row;
                lastColTouched = col;
            }
        }
        #endregion
    }
}