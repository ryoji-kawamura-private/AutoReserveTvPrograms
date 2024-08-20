
using System.ComponentModel.DataAnnotations.Schema;

namespace iEPG.Entity
{
    public class TvProgram : TvProgramBase
    {
		public byte ReserveStatus { get; set; }
		[NotMapped]
		public bool Selected { get; set; }

        public PastTvProgram ConvertToPastTvProgram()
        {
            var pastTvProgram = new PastTvProgram();
            this.CloneTo(pastTvProgram, true);
            return pastTvProgram;
        }
    }
}
