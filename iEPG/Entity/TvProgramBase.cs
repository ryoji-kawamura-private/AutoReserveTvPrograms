using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace iEPG.Entity
{
    public class TvProgramBase
    {
        [Key]
        public Guid Id { get; set; }
        public string ContentType { get; set; }
        public string Version { get; set; }
        public string Station { get; set; }
        public string StationName { get; set; }
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime StartDateTime { get; set; }
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime EndDateTime { get; set; }
        public string ProgramTitle { get; set; }
        public string ProgramSubTitle { get; set; }
        public string Performer { get; set; }
        public string ProgramId { get; set; }
        public string Genre1 { get; set; }
        public string SubGenre1 { get; set; }
        public string Genre2 { get; set; }
        public string SubGenre2 { get; set; }
        public string CopyControl1 { get; set; }
        public string ComponentVideo1 { get; set; }
        public string ComponentAudio1 { get; set; }
        public string Attribute1 { get; set; }
        public string Detail { get; set; }
        [Timestamp]
        public byte[] Timestamp { get; set; }

        private static readonly BindingFlags allBindingFlags = BindingFlags.CreateInstance |
                                                BindingFlags.DeclaredOnly |
                                                BindingFlags.Default |
                                                BindingFlags.ExactBinding |
                                                BindingFlags.FlattenHierarchy |
                                                BindingFlags.GetField |
                                                BindingFlags.GetProperty |
                                                BindingFlags.IgnoreCase |
                                                BindingFlags.IgnoreReturn |
                                                BindingFlags.Instance |
                                                BindingFlags.InvokeMethod |
                                                BindingFlags.NonPublic |
                                                BindingFlags.OptionalParamBinding |
                                                BindingFlags.Public |
                                                BindingFlags.PutDispProperty |
                                                BindingFlags.PutRefDispProperty |
                                                BindingFlags.SetField |
                                                BindingFlags.SetProperty |
                                                BindingFlags.Static |
                                                BindingFlags.SuppressChangeType;

        public void CloneTo(TvProgramBase tvProgramBase, bool copyId)
        {
            var id = tvProgramBase.Id;
            var timestamp = tvProgramBase.Timestamp;
            typeof(TvProgramBase).GetProperties().ToList().ForEach(p =>
            {
                p.SetValue(tvProgramBase, this.GetType().GetProperty(p.Name).GetValue(this, allBindingFlags, null, null, null),
                    allBindingFlags, null, null, null);
            });
            tvProgramBase.Timestamp = timestamp;
            if (!copyId)
            {
                tvProgramBase.Id = id;
            }
        }
    }
}
