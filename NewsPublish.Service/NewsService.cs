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
    public class NewsService
    {
        private Db _db;
        public NewsService(Db db)
        {
            this._db = db;
        }

        /// <summary>
        /// 添加新闻类别
        /// </summary>
        /// <param name="newsClassify"></param>
        /// <returns></returns>
        public ResponseModel AddNewsClassify(AddNewsClassify newsClassify)
        {
            var exit = _db.NewsClassify.FirstOrDefault(c => c.Name == newsClassify.Name) != null;
            if (exit)
            {
                return new ResponseModel { code = 0, result = "该类别已存在" };
            }
            var classify = new NewsClassify { Name = newsClassify.Name, Sort = newsClassify.Sort, Remark = newsClassify.Remark };
            _db.NewsClassify.Add(classify);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "新闻类别添加成功" };
            }
            else
            {
                return new ResponseModel { code = 0, result = "新闻类别添加失败" };
            }
        }

        /// <summary>
        /// 根据id获取新闻类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel GetOneNewsClassify(int id)
        {
            var classify = _db.NewsClassify.Find(id);
            if (classify == null)
            {
                return new ResponseModel { code = 0, result = "该类别不存在" };
            }
            else
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "新闻类别获取成功",
                    data = new NewsClassifyModel
                    {
                        Id = classify.Id,
                        Sort = classify.Sort,
                        Name = classify.Name,
                        Remark = classify.Remark
                    }
                };
            }
        }


        /// <summary>
        /// 根据条件获取新闻类别
        /// </summary>
        /// <returns></returns>
        private NewsClassify GetOneNewsClassify(Expression<Func<NewsClassify, bool>> where)
        {
            return _db.NewsClassify.FirstOrDefault(where);
        }

        /// <summary>
        /// 编辑新闻类别
        /// </summary>
        /// <param name="newsClassify"></param>
        /// <returns></returns>
        public ResponseModel EditNewsClassify(EditNewsClassify newsClassify)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == newsClassify.Id);
            if (classify == null)
            {
                return new ResponseModel { code = 0, result = "该类别不存在" };
            }
            classify.Name = newsClassify.Name;
            classify.Sort = newsClassify.Sort;
            classify.Remark = newsClassify.Remark;
            _db.Update(classify);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "新闻类别编辑成功" };
            }
            else
            {
                return new ResponseModel { code = 0, result = "新闻类别添加编辑失败" };
            }
        }

        /// <summary>
        /// 获取新闻类别列表
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetNewsClassifyList()
        {
            var classifys = _db.NewsClassify.OrderByDescending(c => c.Sort).ToList();
            var response = new ResponseModel { code = 200, result = "新闻类别列表获取成功" };
            response.data = new List<NewsClassifyModel>();
            foreach (var classify in classifys)
            {
                response.data.Add(new NewsClassifyModel
                {
                    Id = classify.Id,
                    Name = classify.Name,
                    Sort = classify.Sort,
                    Remark = classify.Remark
                });
            }
            return response;
        }

        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        public ResponseModel AddNews(AddNews news)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == news.NewsClassifyId);
            if (classify == null)
            {
                return new ResponseModel { code = 0, result = "该类别不存在" };
            }
            var n = new News
            {
                NewsClassifyId = news.NewsClassifyId,
                Title = news.Title,
                Image = news.Image,
                Contents = news.Contents,
                PublishDate = DateTime.Now,
                Remark = news.Remark
            };
            _db.News.Add(n);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "新闻添加成功" };
            }
            else
            {
                return new ResponseModel { code = 0, result = "新闻添加失败" };
            }
        }

        /// <summary>
        /// 根据Id获取一个新闻
        /// </summary>
        /// <param name="NewsId"></param>
        /// <returns></returns>
        public ResponseModel GetOneNews(int NewsId)
        {
            var news = _db.News.Include("NewsClassify").Include("NewsComment").FirstOrDefault(c => c.Id == NewsId);
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "该新闻不存在" };
            }
            else
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "新闻获取成功",
                    data = new NewsModel
                    {
                        Id = news.Id,
                        ClassifyName = news.NewsClassify.Name,
                        Title = news.Title,
                        Image = news.Image,
                        Contents = news.Contents,
                        PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                        CommentCount = news.NewsComment.Count(),
                        Remark = news.Remark
                    }
                };
            }
        }

        /// <summary>
        /// 根据Id删除新闻
        /// </summary>
        /// <param name="NewsId"></param>
        /// <returns></returns>
        public ResponseModel DelOneNews(int NewsId)
        {
            var news = _db.News.FirstOrDefault(c => c.Id == NewsId);
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "该新闻不存在" };
            }
            else
            {
                _db.News.Remove(news);
                int i = _db.SaveChanges();
                if (i > 0)
                {
                    return new ResponseModel { code = 200, result = "新闻删除成功" };
                }
                else
                {
                    return new ResponseModel { code = 0, result = "新闻删除失败" };
                }
            }
        }

        /// <summary>
        /// 分页查询新闻
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel NewsPageQuery(int pageSize, int pageIndex, out int total, List<Expression<Func<News, bool>>> where)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment");
            foreach (var item in where)
            {
                list = list.Where(item);
            }
            total = list.Count();

            var pageData = list.OrderByDescending(c => c.PublishDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "分页新闻获取成功"
            };
            response.data = new List<NewsModel>();
            foreach (var news in pageData)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    ClassifyName = news.NewsClassify.Name,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents.Length>50?news.Contents.Substring(0,50)+"...":news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark
                });
            }
            return response;
        }

        /// <summary>
        /// 查询新闻列表
        /// </summary>
        /// <param name="where"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetNewsList(Expression<Func<News, bool>> where, int topCount)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(where).OrderByDescending(c => c.PublishDate).Take(topCount);

            var response = new ResponseModel
            {
                code = 200,
                result = "新闻列表获取成功"
            };
            response.data = new List<NewsModel>();
            foreach (var news in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    ClassifyName = news.NewsClassify.Name,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents.Length > 50 ? news.Contents.Substring(0, 50) : news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark
                });
            }
            return response;
        }

        /// <summary>
        /// 获取最新评论的新闻集合
        /// </summary>
        /// <param name="where"></param>
        /// <param name="topCount"></param>
        /// <returns></returns>
        public ResponseModel GetNewsCommentNewsList(Expression<Func<News, bool>> where, int topCount)
        {
            var newsIds = _db.NewsComment.OrderByDescending(c => c.AddTime).GroupBy(c => c.NewsId).Select(c => c.Key).Take(topCount);
            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(c => newsIds.Contains(c.Id)).Where(where).OrderByDescending(c => c.PublishDate);
            var response = new ResponseModel
            {
                code = 200,
                result = "最新评论的新闻获取成功"
            };
            response.data = new List<NewsModel>();
            foreach (var news in list)
            {
                response.data.Add(new NewsModel
                {
                    Id = news.Id,
                    ClassifyName = news.NewsClassify.Name,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents.Length > 50 ? news.Contents.Substring(0, 50) : news.Contents,
                    PublishDate = news.PublishDate.ToString("yyyy-MM-dd"),
                    Remark = news.Remark
                });
            }
            return response;
        }

        /// <summary>
        /// 搜索一个新闻
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ResponseModel GetSearchOneNews(Expression<Func<News, bool>> where)
        {
            var news = _db.News.Where(where).FirstOrDefault();
            if (news == null)
            {
                return new ResponseModel
                {
                    code = 0,
                    result = "新闻搜索失败"
                };
            }
            else
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "新闻搜索成功",
                    data = news.Id
                };
            }
        }

        /// <summary>
        /// 获取新闻数量
        /// </summary>
        /// <returns></returns>
        public ResponseModel GetNewsCount(Expression<Func<News, bool>> where)
        {
            var count = _db.News.Where(where).Count();
            return new ResponseModel { code = 200, result = "新闻数量获取成功", data = count };
        }

        /// <summary>
        /// 获取推荐新闻列表
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public ResponseModel GetRecommendNewsList(int newsId)
        {
            var news = _db.News.FirstOrDefault(c => c.Id == newsId);
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "新闻不存在" };
            }
            else
            {
                var newsList = _db.News.Include("NewsComment").Where(c => c.NewsClassifyId == news.NewsClassifyId && c.Id != newsId).OrderByDescending(c => c.PublishDate).OrderByDescending(c => c.NewsComment.Count).Take(6).ToList();
                var response = new ResponseModel
                {
                    code = 200,
                    result = "最新评论的新闻获取成功"
                };
                response.data = new List<NewsModel>();
                foreach (var n in newsList)
                {
                    response.data.Add(new NewsModel
                    {
                        Id = n.Id,
                        ClassifyName = n.NewsClassify.Name,
                        Title = n.Title,
                        Image = n.Image,
                        Contents = n.Contents.Length > 50 ? n.Contents.Substring(0, 50) : n.Contents,
                        PublishDate = n.PublishDate.ToString("yyyy-MM-dd"),
                        Remark = n.Remark
                    });
                }
                return response;
            }

        }
    }
}
