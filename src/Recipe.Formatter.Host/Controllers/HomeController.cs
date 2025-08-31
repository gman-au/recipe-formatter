using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recipe.Formatter.Infrastructure;
using Recipe.Formatter.ViewModel;

namespace Recipe.Formatter.Host.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFormatterEngine _formatterEngine;
        private readonly IQrCodeGenerator _qrCodeGenerator;
        private readonly ITodoistActionGenerator _todoistActionGenerator;

        public HomeController(
            IFormatterEngine formatterEngine,
            IQrCodeGenerator qrCodeGenerator,
            ITodoistActionGenerator todoistActionGenerator)
        {
            _formatterEngine = formatterEngine;
            _qrCodeGenerator = qrCodeGenerator;
            _todoistActionGenerator = todoistActionGenerator;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Process(RecipeParseRequestViewModel value)
        {
            var view = value.Style ?? "Basic";

            string status = null;

            try
            {
                var response =
                    await
                        _formatterEngine
                            .ProcessAsync(value);

                if (response.Success)
                {
                    var todoistActionValue =
                        _todoistActionGenerator.Generate(
                            response.Recipe.Title,
                            response?.Recipe?.Ingredients ?? []);

                    var qrCode =
                        await
                            _qrCodeGenerator
                                .GenerateAsync(todoistActionValue);

                    if (!string.IsNullOrEmpty(qrCode))
                        response.QrCodeBase64 = $"data:image/png;base64,{qrCode}";

                    ViewBag.PageTitle = $"{response.Recipe?.Title} - ({view})";
                    return View(view, response);
                }

                return View("Index", response.Status);
            }
            catch (Exception ex)
            {
                return View("Index", new StatusViewModel {Message = status ?? ex.Message, Url = value.Url});
            }
        }
    }
}
