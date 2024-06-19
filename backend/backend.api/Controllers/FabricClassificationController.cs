using backend.api.Models;
using backend.api.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FabricClassificationController : ControllerBase
    {
        private readonly PredictionEnginePool<FabricModelInput, FabricModelOutput> _predictionEnginePool;
        private const double ConfidenceThreshold = 75.0; // Umbral de confianza del 75%
        public FabricClassificationController(PredictionEnginePool<FabricModelInput, FabricModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpPost]
        public ActionResult Post([FromForm] ImageClassificationRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            byte[] byteArray;
            using (var ms = new MemoryStream())
            {
                request.Image.CopyTo(ms);
                byteArray = ms.ToArray();
            }

            var input = new FabricModelInput
            {
                ImageSource = byteArray
            };

            var predictionResult = _predictionEnginePool.Predict(modelName: "ClasificacionImagen.MLModels.FabricMLModel", example: input);
            var confidence = Math.Round(predictionResult.Score.Max() * 100, 2);

            if (confidence < ConfidenceThreshold) // Establece tu umbral de confianza aquí
            {
                return BadRequest( "La tela no pudo ser reconocida. Por favor, inténtalo de nuevo con otra imagen.");
            }

            return Ok(new
            {
                tela = predictionResult.PredictedLabel,
                //confidence = confidence
            });
        }
    }
}
