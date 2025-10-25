using Domain.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ArticleTagModel : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public virtual ICollection<ArticleModel> Articles { get; set; } = new List<ArticleModel>();
    }
}
