using Microsoft.EntityFrameworkCore;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NewsPublish.Service
{
    public class CommentService
    {
        private Db _db;
        private NewsService _newsService;
        public CommentService(Db db,NewsService newsService)
        {
            this._db = db;
            this._newsService = newsService;
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public ResponseModel AddComment(AddComment comment)
        {
            var news = _newsService.GetOneNews(comment.NewsId);
            if(news.code==0)
            {
                return new ResponseModel { code = 0, result = "新闻不存在" };
            }
            var com = new NewsComment { NewsId = comment.NewsId, AddTime = DateTime.Now, Contents = comment.Contents };
            _db.NewsComment.Add(com);
            int i = _db.SaveChanges();
            if(i>0)
            {

                return new ResponseModel { code = 200, result = "新闻评论添加成功",data=new  { contents = comment.Contents, floor = "#" + Convert.ToInt32(news.data.CommentCount) + 1, addTime = DateTime.Now } };
            }
            {
                return new ResponseModel { code = 0, result = "新闻评论添加失败" };
            }
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteComment(int id)
        {
            var comment = _db.NewsComment.Find(id);
            if(comment==null)
            {
                return new ResponseModel { code = 0, result = "评论不存在" };
            }
            _db.NewsComment.Remove(comment);
            int i = _db.SaveChanges();
            if(i>0)
            {
                return new ResponseModel { code = 200, result = "新闻评论删除成功" };
            }
            else
            {
                return new ResponseModel { code = 0, result = "新闻评论删除失败" };
            }
        }

        /// <summary>
        /// 获取评论集合
        /// </summary>
        /// <param name="where"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetCommentList(Expression<Func<NewsComment,bool>> where,int topCount)
        {
            var comments = _db.NewsComment.Include("News").Where(where).OrderBy(c => c.AddTime).ToList();
            var response = new ResponseModel();
            response.code = 200;
            response.result = "评论获取成功";
            response.data = new List<CommentModel>();
            int floor = 1;
            foreach (var comment in comments)
            {
                response.data.Add(new CommentModel {
                    Id=comment.Id,
                    Contents=comment.Contents,
                    AddTime=comment.AddTime,
                    NewsName=comment.News.Title,
                    Remark=comment.Remark,
                    Floor="#"+floor,
                });
                floor++;
            }
            response.data.Reverse();
            return response;
        }
    }
}
