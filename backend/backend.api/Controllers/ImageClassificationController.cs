using backend.api.Models;
using backend.api.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;

namespace backend.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageClassificationController : ControllerBase
    {
        private readonly PredictionEnginePool<FabricModelInput, FabricModelOutput> _predictionEnginePool;
        private const double ConfidenceThreshold = 75.0;
        public ImageClassificationController(PredictionEnginePool<FabricModelInput, FabricModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool ?? throw new ArgumentNullException(nameof(predictionEnginePool));
        }

        /// <summary>
        /// Classifies an image to predict the fabric type.
        /// </summary>
        /// <param name="request">The image classification request model.</param>
        /// <returns>The prediction result.</returns>
        /// <response code="200">Returns the predicted fabric type.</response>
        /// <response code="400">If the image could not be classified.</response>
        [Authorize(Roles = "usuario")]
        [HttpPost]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public ActionResult Post([FromForm] ImageClassificationRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var predictionResult = Predict(request.Image);
            var confidence = Math.Round(predictionResult.Score.Max() * 100, 2);

            if (confidence < ConfidenceThreshold)
                return BadRequest( "La tela no pudo ser reconocida. Por favor, inténtalo de nuevo con otra imagen.");

            return Ok(new
            {
                tela = predictionResult.PredictedLabel,
            });
        }

        private FabricModelOutput Predict(IFormFile image)
        {
            byte[] byteArray;

            using (var ms = new MemoryStream())
            {
                image.CopyTo(ms);
                byteArray = ms.ToArray();
            }

            var input = new FabricModelInput
            {
                ImageSource = byteArray
            };

            var predictionResult = _predictionEnginePool.Predict(modelName: "ClasificacionImagen.MLModels.FabricMLModel", example: input);

            return predictionResult;
        }
    }
}
