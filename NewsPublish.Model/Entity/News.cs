using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPublish.Model.Entity
{
    public class News
    {
        public News()
        {
            this.NewsConment = new HashSet<NewsConment>();
        }
        public int Id { get; set; }
        public int NewsClassifyId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Contents { get; set; }
        public DateTime PublishDate { get; set; }
        public string Remark { get; set; }
        public virtual NewsClassify NewsClassify { get; set; }
        public virtual ICollection<NewsConment> NewsConment { get; set; }
    }
}
