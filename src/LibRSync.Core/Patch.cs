using System.IO;

namespace LibRSync.Core
{
    internal class Patch : Job
    {
        private readonly Stream @base;
        private readonly Stream delta;
        private readonly Stream @new;

        public Patch(Stream @base, Stream delta, Stream @new) : base("patch")
        {
            this.@base = @base;
            this.delta = delta;
            this.@new = @new;
        }

        protected override StateFunc InitialState()
        {
            return Completed;
        }
    }
}
