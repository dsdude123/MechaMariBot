using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechaMariBot.Models
{
    public class ReactionTrigger
    {
        public string[] triggerWords { get; set; }
        public ulong[] triggerUsers { get; set; }
        public string reactionEmote { get; set; }

        public ReactionTrigger(string[] triggerWords, ulong[] triggerUsers, string reactionEmote)
        {
            this.triggerWords = triggerWords;
            this.triggerUsers = triggerUsers;
            this.reactionEmote = reactionEmote;
        }
    }
}
