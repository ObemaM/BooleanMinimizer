using BooleanMinimizator.Models;
using BooleanMinimizerLibrary;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BooleanMinimizator.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new BooleanMinimizatorModel());
        }

        public IActionResult Index(BooleanMinimizatorModel model)
        {
            if (!string.IsNullOrEmpty(model.InputFunction))
            {
                try
                {
                    // Проверка длины с учетом Unicode символов
                    if (model.InputFunction.Length > 64 || model.InputFunction.ToCharArray().Length > 64)
                    {
                        model.ResultMessage = "Ошибка: Длина ввода не может превышать 64 символа";
                        model.IsSolved = false;
                        return View(model);
                    }

                    var syntaxAnalyzer = new SyntaxAnalyzer();
                    Node rootNode = syntaxAnalyzer.Parse(model.InputFunction);

                    var functionVectorBuilder = new FunctionVectorBuilder();
                    model.VectorOutput = functionVectorBuilder.BuildVector(rootNode);
                    model.TruthTable = functionVectorBuilder.BuildTruthTable(rootNode);
                    model.MDNFOutput = BooleanMinimizer.MinimizeSDNF(model.VectorOutput);
                    model.MKNFOutput = BooleanMinimizer.MinimizeSKNF(model.VectorOutput);
                    model.SKNFOutput = BooleanMinimizer.GetFullSKNF(model.VectorOutput);
                    model.SDNFOutput = BooleanMinimizer.GetFullSDNF(model.VectorOutput);

                    // Добавлено построение карты Карно
                    var karnaughBuilder = new KarnaughMapBuilder();
                    model.KarnaughSteps = karnaughBuilder.BuildSteps(rootNode);
                    model.KarnaughMap = model.KarnaughSteps.Last().Map; // Итоговая карта
                    model.KarnaughMap = karnaughBuilder.Build(rootNode);

                    model.Variables = karnaughBuilder.GetVariablesFromMap(model.KarnaughMap);
                    model.Areas = karnaughBuilder.FindAllMaximalAreas(model.KarnaughMap);
                    model.ZeroAreas = karnaughBuilder.FindAllMaximalZeroAreas(model.KarnaughMap);

                    model.ResultMessage = "Функция успешно распознана!";
                    model.IsSolved = true;

                    model.ResultMessage = "Функция успешно распознана!";
                }
                catch (Exception ex)
                {
                    model.ResultMessage = $"{ex.Message}";
                    model.IsSolved = false;
                }
            }
            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}