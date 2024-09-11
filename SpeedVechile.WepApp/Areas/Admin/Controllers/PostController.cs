using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SpeedVechile.Application.ApplicationConstants;
using SpeedVechile.Application.Contracts.Presistence;
using SpeedVechile.Application.Services.Interface;
using SpeedVechile.Domain.ApplicationEnums;
using SpeedVechile.Domain.Models;
using SpeedVechile.Domain.ViewModel;
using SpeedVechile.Infrastructure.Comman;

namespace SpeedVechile.WepApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CustomRole.MasterAdmin + "," + CustomRole.Admin)]

    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserNameService _userName;

        public PostController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment,IUserNameService userName)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userName = userName;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Post> posts = await _unitOfWork.Post.GetAllPost();
            return View(posts);
        }
        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> engineAndFuelType = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            IEnumerable<SelectListItem> tranmission = Enum.GetValues(typeof(Transmission)).Cast<Transmission>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });
            PostVM postVM = new PostVM
            {
                Post=new Post(),
                BrandList=brandList,
                VehicleTypeList=vehicleTypeList,
                EngineAndFuelTypeList=engineAndFuelType,
                TranmissionList=tranmission
            };

            return View(postVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;
            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\post");

                var extension = Path.GetExtension(file[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                postVM.Post.VehicleImage = @"\images\post\" + newFileName + extension;
            }
            if (ModelState.IsValid)
            {
                await _unitOfWork.Post.Create(postVM.Post);
                await _unitOfWork.SaveAsync();

                TempData["success"] = CommanMessage.RecordCreated;

                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Post post = await _unitOfWork.Post.GetPostById(id);

            post.CreatedBy = await _userName.GetUserName(post.CreatedBy);
            post.ModifiedBy = await _userName.GetUserName(post.ModifiedBy);
            return View(post);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Post post = await _unitOfWork.Post.GetPostById(id);

            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> engineAndFuelType = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            IEnumerable<SelectListItem> tranmission = Enum.GetValues(typeof(Transmission)).Cast<Transmission>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });
            PostVM postVM = new PostVM
            {
                Post = post,
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                EngineAndFuelTypeList = engineAndFuelType,
                TranmissionList = tranmission
            };
            return View(postVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;
            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\post");

                var extension = Path.GetExtension(file[0].FileName);

                //Delete Old Image

                var objFromDb = await _unitOfWork.Post.GetByIdAsync(postVM.Post.Id);

                if (objFromDb.VehicleImage != null)
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.VehicleImage.Trim('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                postVM.Post.VehicleImage = @"\images\post\" + newFileName + extension;
            }
            if (ModelState.IsValid)
            {
                await _unitOfWork.Post.Update(postVM.Post);
                await _unitOfWork.SaveAsync();

                TempData["warning"] = CommanMessage.RecordUpdated;

                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Post post = await _unitOfWork.Post.GetByIdAsync(id);
            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> engineAndFuelType = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            IEnumerable<SelectListItem> tranmission = Enum.GetValues(typeof(Transmission)).Cast<Transmission>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });
            PostVM postVM = new PostVM
            {
                Post = post,
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                EngineAndFuelTypeList = engineAndFuelType,
                TranmissionList = tranmission
            };

            return View(postVM);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            if (!string.IsNullOrEmpty(postVM.Post.VehicleImage))
            {
                //Delete Old Image

                var objFromDb = await _unitOfWork.Post.GetByIdAsync(postVM.Post.Id);

                if (objFromDb.VehicleImage != null)
                {
                    var oldImagePath = Path.Combine(webRootPath, objFromDb.VehicleImage.Trim('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                }
            }
            await _unitOfWork.Post.Delete(postVM.Post);
            await _unitOfWork.SaveAsync();

            TempData["error"] = CommanMessage.RecordDeleted;

            return RedirectToAction(nameof(Index));


        }
    }
}
