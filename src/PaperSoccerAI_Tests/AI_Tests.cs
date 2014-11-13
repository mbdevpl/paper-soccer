using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PaperSoccerAI;

namespace PaperSoccerAI_Tests {

    [TestClass]
    public class AI_Tests {

        [TestMethod]
        public void GameBoard_AddMove_Test() {
            GameBoard gb = new GameBoard();
            gb.AddMove(Direction.UpRight);
            gb.AddMove(Direction.Right);
            gb.AddMove(Direction.DownRight);
            gb.AddMove(Direction.Down);
            gb.AddMove(Direction.DownLeft);
            gb.AddMove(Direction.Left);
            gb.AddMove(Direction.UpLeft);
            gb.AddMove(Direction.Up);
        }

        [TestMethod]
        public void AI_Construction_Test() {
            GameBoard gb = new GameBoard();
            //AIEngine ai = new AIEngine(gb);
        }

    }

}
