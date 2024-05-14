using backend.api.Models;
using backend.api.Validadores;

namespace backend.api.test
{
    public class UsuarioRequestModelValidatorTests
    {
        private UsuarioRequestModelValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new UsuarioRequestModelValidator();
        }

        [Test]
        public void Validate_WhenNombreIsEmpty_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { Nombre = "" };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "Nombre" && e.ErrorMessage.Contains("obligatorio")), Is.True);
            });
        }

        [Test]
        public void Validate_WhenEmailIsInvalid_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { Email = "invalidemail" };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "Email" && e.ErrorMessage.Contains("válido")), Is.True);
            });
        }

        [Test]
        public void Validate_WhenTelefonoIsTooShort_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { Telefono = "1234567" };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "Telefono" && e.ErrorMessage.Contains("8 dígitos")), Is.True);
            });
        }

        [Test]
        public void Validate_WhenRolIdIsInvalid_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { RolId = 3 };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "RolId" && e.ErrorMessage.Contains("inválido")), Is.True);
            });
        }

        [Test]
        public void Validate_WhenPasswordIsTooShort_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { Password = "12345" };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "Password" && e.ErrorMessage.Contains("6 caracteres")), Is.True);
            });
        }

        [Test]
        public void Validate_WhenDireccionIsEmpty_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { Direccion = "" };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "Direccion" && e.ErrorMessage.Contains("no puede estar vacía")), Is.True);
            });
        }

        [Test]
        public void Validate_WhenLocalidadIsEmpty_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { Localidad = "" };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "Localidad" && e.ErrorMessage.Contains("no puede estar vacía")), Is.True);
            });
        }

        [Test]
        public void Validate_WhenProvinciaIsEmpty_ShouldBeInvalid()
        {
            var model = new UsuarioRequestModel { Provincia = "" };
            var result = _validator.Validate(model);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Any(e => e.PropertyName == "Provincia" && e.ErrorMessage.Contains("no puede estar vacía")), Is.True);
            });
        }

    }
}
