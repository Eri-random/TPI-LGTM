using backend.servicios.Config;
using backend.servicios.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace backend.servicios.Servicios
{
    /// <summary>
    /// Service for interacting with the Groq API to generate ideas.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GroqApiService"/> class.
    /// </remarks>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="config">The Groq API configuration settings.</param>
    /// <exception cref="ArgumentNullException">Thrown if httpClientFactory or config is null.</exception>
    public class GroqApiService(IHttpClientFactory httpClientFactory, GroqApiConfig config) : IGenerateIdeaApiService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private readonly GroqApiConfig _config = config ?? throw new ArgumentNullException(nameof(config));

        /// <summary>
        /// Generates an idea based on the provided user message.
        /// </summary>
        /// <param name="userMessage">The message from the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated idea as a string.</returns>
        public async Task<string> GenerateIdea(string userMessage)
        {
            var httpClient = GetHttpClient();
            var httpRequestContent = GetHttpMessageContent(userMessage);
            var response = await httpClient.PostAsync(_config.RequestUri, httpRequestContent);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Creates and configures an HTTP client.
        /// </summary>
        /// <returns>A configured HTTP client instance.</returns>
        private HttpClient GetHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

            return httpClient;
        }

        /// <summary>
        /// Creates the HTTP message content for the API request.
        /// </summary>
        /// <param name="userMessage">The message from the user.</param>
        /// <returns>A <see cref="StringContent"/> instance containing the serialized request body.</returns>
        private StringContent GetHttpMessageContent(string userMessage)
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = GetSystemMessage()
                    },
                    new
                    {
                        role = "user",
                        content = userMessage
                    }
                },
                model = _config.LlmModel,
                temperature = 0.6,
                max_tokens = 5000,
                top_p = 1,
                stream = false,
                response_format = new
                {
                    type = "json_object"
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpMessageContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            return httpMessageContent;
        }

        /// <summary>
        /// Gets the system message to be included in the API request.
        /// </summary>
        /// <returns>A string containing the system message.</returns>
        //    private static string GetSystemMessage() =>
        //        "Tu tarea es generar una nueva idea de reciclaje utilizando sobrantes o retazos de tela, con los pasos detallados para llevarla a cabo.\n\n" +
        //        "La idea tiene que ser real, adecuada para las dimensiones y tipos de tela proporcionados. Necesito que sean productos para donar a una organizacion benefica y estos a su vez puedan donarselo a gente carenciada\n\n" +
        //        "Recuerda que solo puedes responder en español. \n\n" +
        //        "Usa los retazos entregados para generar la idea, debe ser acorde a las dimensiones de los retazos. No propongas ideas que requieran más tela de la proporcionada.\n\n" +
        //        "La respuesta final debe estar únicamente en formato JSON y debe contener:\n\n" +
        //        "{\n  " +
        //            "\"Idea\": \"Descripción breve de la nueva idea\",\n  \"Pasos\": [\n    \"Paso 1: Descripción del paso 1\",\n    \"Paso 2: Descripción del paso 2\",\n    \"Paso 3: Descripción del paso 3\",\n    ...\n  ]\n}\n" +
        //        "Solo devuelve el JSON como respuesta final, no agregues nada más.\n\n" +
        //        "Ejemplo de respuesta:\n" +
        //        "{\n  " +
        //            "\"Idea\": \"Llavero de tela reciclada\",\n  \"Pasos\": [\n    \"Paso 1: Elige dos piezas de tela reciclada, preferiblemente con colores y diseños complementarios. Utiliza una plantilla en forma de corazón para marcar la forma en ambas piezas de tela.\",\n    \"Paso 2: Recorta las dos piezas de tela siguiendo las líneas marcadas. Asegúrate de que ambas piezas sean del mismo tamaño para que coincidan perfectamente al coser.\",\n    \"Paso 3: Cose alrededor del borde de las piezas, dejando una pequeña abertura de aproximadamente 3 cm en un lado del corazón. Esto te permitirá darle la vuelta a la tela y rellenar el corazón más adelante.\",\n    \"Paso 4: Finalmente, añade un aro de llavero a la parte superior del corazón. Puedes coser una pequeña tira de tela en la parte superior del corazón y pasar el aro a través de ella, o usar un gancho de llavero si prefieres una opción removible.\"\n  ]\n}\n" +
        //        "Otro ejemplo de respuesta:\n" +
        //        "{\n  " +
        //            "\"Idea\": \"Brazalete de tela\",\n  \"Pasos\": [\n    \"Paso 1: Elige una tela que te guste y corta una tira de aproximadamente 10 cm de largo y 2 cm de ancho. Asegúrate de que los bordes sean rectos para facilitar el siguiente paso. Si deseas un brazalete más ancho o más estrecho, ajusta las medidas según tu preferencia.\",\n    \"Paso 2: Dobla la tira de tela por la mitad a lo largo, de modo que los lados derechos (los que quieres que queden visibles) estén enfrentados. Usa alfileres para mantener los bordes alineados y evitar que la tela se desplace mientras coses.\",\n    \"Paso 3: Cose a lo largo del borde abierto de la tira, dejando un margen de costura pequeño (alrededor de 0.5 cm). Cuando termines de coser, voltea la tira del revés al derecho para que la costura quede en el interior. Puedes usar un lápiz o un palillo para ayudarte a darle la vuelta a la tela.\",\n    \"Paso 4: Plancha la tira de tela para aplanarla y darle un acabado más pulido. Luego, añade un broche en cada extremo del brazalete para cerrarlo. Puedes usar broches a presión, botones, o cualquier otro tipo de cierre que prefieras. Asegúrate de que estén bien fijados para que el brazalete no se abra mientras lo llevas puesto.\"\n  ]\n}\n";
        //}

        private static string GetSystemMessage() =>
        "Tu tarea es generar una nueva y única idea de reciclaje utilizando sobrantes o retazos de tela, con los pasos detallados para llevarla a cabo.\n\n" +
        "La idea debe ser original, realista y adecuada para las dimensiones y tipos de tela proporcionados. El objetivo es crear productos para donar a una organización benéfica, que a su vez los entregará a personas necesitadas.\n\n" +
        "Recuerda que solo puedes responder en español.\n\n" +
        "Usa los retazos entregados para generar la idea, debe ser acorde a las dimensiones de los retazos. No propongas ideas que requieran más tela de la proporcionada. Asegúrate de variar las ideas en cada solicitud, evitando repetir las mismas ideas con diferentes tipos de tela.\n\n" +
        "La respuesta final debe estar únicamente en formato JSON y debe contener:\n\n" +
        "{\n  " +
            "\"Idea\": \"Descripción breve de la nueva idea\",\n  \"Dificultad\": \"Nivel de dificultad para llevar a cabo la idea\",\n  \"Pasos\": [\n    \"Paso 1: Descripción detallada del paso 1\",\n    \"Paso 2: Descripción detallada del paso 2\",\n    \"Paso 3: Descripción detallada del paso 3\",\n    ...\n  ]\n}\n" +
        "Solo devuelve el JSON como respuesta final, no agregues nada más.\n\n" +
        "Ejemplo de respuesta:\n" +
        "{\n  " +
            "\"Idea\": \"Buzo reciclado de retazos de tela\",\n  \"Dificultad\": \"Alta\",\n  \"Pasos\": [\n    \"Paso 1: Selecciona varios retazos de tela que sean de materiales y grosores similares. Es importante que los colores y patrones combinen de una manera armoniosa. Mide y corta las piezas en cuadrados o rectángulos de tamaño uniforme (por ejemplo, 20x20 cm).\",\n    \"Paso 2: Organiza los retazos de tela en el diseño que prefieras para el buzo. Asegúrate de tener suficientes piezas para cubrir el frente, la espalda, las mangas y la capucha si deseas agregar una. Es útil tomar una prenda de buzo existente como guía para las dimensiones generales.\",\n    \"Paso 3: Cose los retazos juntos siguiendo el diseño que has creado, utilizando una puntada recta y un margen de costura de aproximadamente 1 cm. Comienza con las piezas más grandes para formar el cuerpo del buzo y luego une las mangas y otros detalles. Recuerda planchar las costuras para que queden planas y prolijas antes de continuar.\",\n    \"Paso 4: Una vez que todas las piezas estén cosidas y planchadas, une las partes principales del buzo (frente, espalda y mangas). Asegúrate de reforzar las costuras en áreas que recibirán más tensión, como las axilas y los hombros.\",\n    \"Paso 5: Si deseas agregar una capucha, corta y cose las piezas necesarias, luego únelas al cuello del buzo. Finaliza los bordes de las mangas y la parte inferior del buzo con dobladillos o puños elásticos para un mejor ajuste.\",\n    \"Paso 6: Realiza un acabado final revisando todas las costuras y asegurándote de que no haya hilos sueltos. Plancha el buzo completo para darle una apariencia pulida y profesional.\"\n  ]\n}\n" +
        "Otro ejemplo de respuesta:\n" +
        "{\n  " +
            "\"Idea\": \"Campera de retazos de tela reciclada\",\n  \"Dificultad\": \"Media\",\n  \"Pasos\": [\n    \"Paso 1: Selecciona varios retazos de tela que sean adecuados para una campera, asegurándote de que sean de materiales duraderos y colores que combinen bien. Mide y corta las piezas en formas que se adapten a las secciones de una campera (cuerpo, mangas, bolsillos).\",\n    \"Paso 2: Organiza los retazos en el diseño que prefieras para la campera. Es útil tener una campera existente como referencia para las dimensiones y la disposición de las piezas. Cose los retazos juntos utilizando una puntada recta y un margen de costura adecuado.\",\n    \"Paso 3: Una vez que las piezas principales estén unidas (frente, espalda, mangas), comienza a ensamblar la campera. Primero une el frente y la espalda por los hombros y los lados. Luego, agrega las mangas, asegurándote de que las costuras estén alineadas y reforzadas.\",\n    \"Paso 4: Cose los detalles adicionales como bolsillos, capucha, o parches decorativos. Si la campera incluye una cremallera, asegúrate de coserla cuidadosamente para que quede recta y funcione correctamente.\",\n    \"Paso 5: Finaliza los bordes de las mangas y el dobladillo inferior con una puntada de dobladillo o añadiendo ribetes elásticos para un ajuste más cómodo y profesional.\",\n    \"Paso 6: Revisa todas las costuras y refuerza las áreas que podrían estar sujetas a mayor desgaste. Plancha toda la campera para que las costuras queden planas y el acabado sea más prolijo.\"\n  ]\n}\n";

    }

}
