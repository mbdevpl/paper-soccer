using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperSoccerAI {

	class RatedGamePoint : GamePoint {

		public Double UpRightRating;

		public Double RightRating;

		public Double DownRightRating;

		public Double DownRating;

		public Double DownLeftRating;

		public Double LeftRating;

		public Double UpLeftRating;

		public Double UpRating;

		private void InitDefault() {
			Double defaultVal = 0.5;

			UpRightRating = defaultVal;
			RightRating = defaultVal;
			DownRightRating = defaultVal;
			DownRating = defaultVal;

			DownLeftRating = defaultVal;
			LeftRating = defaultVal;
			UpLeftRating = defaultVal;
			UpRating = defaultVal;
		}

		public RatedGamePoint()
			: base() {
			InitDefault();
		}

		public RatedGamePoint(GamePoint pt)
			: base(pt) {
			InitDefault();
		}

		public RatedGamePoint(RatedGamePoint pt)
			: base(pt) {
			UpRightRating = pt.UpRightRating;
			RightRating = pt.RightRating;
			DownRightRating = pt.DownRightRating;
			DownRating = pt.DownRating;

			DownLeftRating = pt.DownLeftRating;
			LeftRating = pt.LeftRating;
			UpLeftRating = pt.UpLeftRating;
			UpRating = pt.UpRating;
		}

	}

}
