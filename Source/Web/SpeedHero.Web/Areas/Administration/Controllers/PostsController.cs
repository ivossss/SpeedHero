﻿namespace SpeedHero.Web.Areas.Administration.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Kendo.Mvc.Extensions;
    using Kendo.Mvc.UI;

    using SpeedHero.Data.Common.Repositories;
    using SpeedHero.Data.Models;
    using SpeedHero.Web.Areas.Administration.Controllers.Base;
    using SpeedHero.Web.Areas.Administration.ViewModels.Posts;
    using SpeedHero.Web.Helpers;

    public class PostsController : AdminController
    {
        private readonly IDeletableEntityRepository<Post> postsRepository;

        public PostsController(IDeletableEntityRepository<Post> postsDeletableRepository)
        {
            this.postsRepository = postsDeletableRepository;
        }

        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost] // kendo sends data for filtering, paging, sorting
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var posts = this.postsRepository
                .All()
                .ProjectTo<ShowPostsViewModel>();

            DataSourceResult result = posts.ToDataSourceResult(request);

            return this.Json(result);
        }

        [HttpPost]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, UpdatePostViewModel inputPost)
        {
            Post postFromDatabase = null;

            if (ModelState.IsValid)
            {
                postFromDatabase = this.postsRepository
                    .GetById(inputPost.Id);
                Mapper.Map(inputPost, postFromDatabase);
                this.postsRepository.Update(postFromDatabase);
                this.postsRepository.SaveChanges();
            }

            var modifedPostForKendo = Mapper.Map<ShowPostsViewModel>(postFromDatabase);

            return this.Json(new[] { modifedPostForKendo }.ToDataSourceResult(request, this.ModelState));
        }

        [HttpPost]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, DeletePostViewModel inputPost)
        {
            if (ModelState.IsValid)
            {
                this.postsRepository.Delete(inputPost.Id);
                this.postsRepository.SaveChanges();
            }

            return this.Json(new[] { inputPost }.ToDataSourceResult(request, this.ModelState));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var postFromDatabase = this.postsRepository
                .GetById(id.Value);

            if (postFromDatabase == null)
            {
                return this.HttpNotFound("Post not found");
            }

            var mappedPost = Mapper.Map<PostDetailsViewModel>(postFromDatabase);
            return this.View(mappedPost);
        }

        public ActionResult Edit(int? id, string returnUrl)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var postFromDatabase = this.postsRepository
                .GetById(id.Value);

            if (postFromDatabase == null)
            {
                return this.HttpNotFound("Post not found");
            }

            ViewBag.ReturnUrl = returnUrl;
            var mappedPost = Mapper.Map<EditPostViewModel>(postFromDatabase);
            return this.View(mappedPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditPostViewModel inputPost, string returnUrl)
        {
            if (inputPost.File != null && !KendoUpload.CheckIsFileAnImage(inputPost.File))
            {
                ModelState.AddModelError("Cover photo", "Cover photo must be of type \"jpeg\" or \"png\".");
            }

            if (ModelState.IsValid)
            {
                var postFromDatabase = this.postsRepository
                    .GetById(inputPost.Id);
                Mapper.Map(inputPost, postFromDatabase);

                if (inputPost.File != null)
                {
                    postFromDatabase.CoverPhotoPath = WebConstants.ImagesPath + inputPost.File.FileName;
                    KendoUpload.SaveCoverPhoto(inputPost.File, WebConstants.ImagesPath, this.Server);
                }

                this.postsRepository.Update(postFromDatabase);
                this.postsRepository.SaveChanges();

                return this.Redirect(returnUrl);
            }

            ViewBag.ModelState = "Invalid";
            return this.View(inputPost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            this.postsRepository.Delete(id.Value);
            this.postsRepository.SaveChanges();

            return this.RedirectToAction("Index", "Home", new { area = string.Empty });
        }
    }
}