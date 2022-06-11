using DiscordYoutubeNotify.EfCore.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordYoutubeNotify.Models
{
    public class Subscription: IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string DiscordChannelId { get; set; }

        public Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
    }
}
