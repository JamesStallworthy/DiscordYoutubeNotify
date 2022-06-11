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
    public class Channel: IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string? LatestVideoId { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}
