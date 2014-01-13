using System.IO;

namespace LibRSync.Core
{
    class Delta : Job
    {
        private Stream signature;
        private Stream @new;
        private Stream delta;

        public Delta(Stream signature, Stream @new, Stream delta)
            : base("delta")
        {
            this.signature = signature;
            this.@new = @new;
            this.delta = delta;
        }

        protected override StateFunc InitialState()
        {
            return Completed;
        }
    }
}
