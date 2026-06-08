using UndirLaFlota.Juego;

namespace UndirLaFlota
{
    class GameManager
    {
        #region Variables
        public Tablero BoardPlayer01 { get; private set; }
        public Tablero BoardPlayer02 { get; private set; }
        public IAPlayer EnemigoIA { get; private set; }
        public bool IsPlayerTurn { get; set; }
        #endregion

        #region Constructor de GameManager
        public GameManager()
        {
            BoardPlayer01 = new Tablero(false);
            BoardPlayer02 = new Tablero(true);
            EnemigoIA = new IAPlayer();
            IsPlayerTurn = false;
        }
        #endregion

        #region Método Propios
        /// <summary>
        /// Método que gestiona si el jugador tiene el turno de juego o no.
        /// </summary>
        /// <param name="boardAtacado"></param>
        /// <returns></returns>
        public bool PlayerCanShoot(Tablero boardAtacado)
        {
            if (BoardPlayer01.IsPlacingBoats) return false;
            if (!IsPlayerTurn) return false;
            if (boardAtacado == BoardPlayer01) return false;

            return true;
        }

        //public string? RegistrarDisparoJugador(int row, int col)
        //{
        //    return BoardPlayer02.Jugada(row, col);
        //}
        #endregion
    }
}
