using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.SalaryStructure;

namespace HRMS.Tests.Services;

[TestFixture]
public class SalaryStructureServiceTests
{
private Mock<ISalaryStructureRepository>
repository;


private SalaryStructureService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<ISalaryStructureRepository>();

    service =
        new SalaryStructureService(
            repository.Object);
}

[Test]
public void Create_ShouldCreateSalaryStructure_WhenValid()
{
    var dto =
        new CreateSalaryStructureDto
        {
            Name = "Default",

            BasicPercentage = 40,

            HraPercentage = 20,

            SpecialAllowancePercentage = 20,

            MedicalAllowancePercentage = 10,

            TravelAllowancePercentage = 10
        };

    repository
        .Setup(x =>
            x.GetByName(
                dto.Name))
        .Returns(
            (SalaryStructure?)null);

    service.Create(dto);

    repository.Verify(
        x => x.Add(
            It.IsAny<SalaryStructure>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void Create_ShouldThrowBusinessException_WhenPercentagesDoNotTotal100()
{
    var dto =
        new CreateSalaryStructureDto
        {
            Name = "Default",

            BasicPercentage = 40,

            HraPercentage = 20,

            SpecialAllowancePercentage = 20,

            MedicalAllowancePercentage = 10,

            TravelAllowancePercentage = 5
        };

    Assert.Throws<
        BusinessException>(
        () =>
            service.Create(dto));
}

[Test]
public void Create_ShouldThrowBusinessException_WhenSalaryStructureAlreadyExists()
{
    repository
        .Setup(x =>
            x.GetByName(
                It.IsAny<string>()))
        .Returns(
            new SalaryStructure());

    Assert.Throws<
        BusinessException>(
        () =>
            service.Create(
                new CreateSalaryStructureDto
                {
                    Name = "Default",

                    BasicPercentage = 40,

                    HraPercentage = 20,

                    SpecialAllowancePercentage = 20,

                    MedicalAllowancePercentage = 10,

                    TravelAllowancePercentage = 10
                }));
}

[Test]
public void Update_ShouldUpdateSalaryStructure_WhenValid()
{
    var structure =
        new SalaryStructure
        {
            Id = Guid.NewGuid(),

            Name = "Old"
        };

    repository
        .Setup(x =>
            x.GetById(
                structure.Id))
        .Returns(
            structure);

    var dto =
        new UpdateSalaryStructureDto
        {
            Name = "Updated",

            BasicPercentage = 50,

            HraPercentage = 20,

            SpecialAllowancePercentage = 10,

            MedicalAllowancePercentage = 10,

            TravelAllowancePercentage = 10
        };

    service.Update(
        structure.Id,
        dto);

    Assert.That(
        structure.Name,
        Is.EqualTo(
            "Updated"));

    Assert.That(
        structure.BasicPercentage,
        Is.EqualTo(50));

    repository.Verify(
        x => x.Update(
            structure),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void Update_ShouldThrowNotFoundException_WhenSalaryStructureNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (SalaryStructure?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.Update(
                Guid.NewGuid(),
                new UpdateSalaryStructureDto()));
}

[Test]
public void Update_ShouldThrowBusinessException_WhenPercentagesDoNotTotal100()
{
    var structure =
        new SalaryStructure
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                structure.Id))
        .Returns(
            structure);

    Assert.Throws<
        BusinessException>(
        () =>
            service.Update(
                structure.Id,
                new UpdateSalaryStructureDto
                {
                    Name = "Updated",

                    BasicPercentage = 40,

                    HraPercentage = 20,

                    SpecialAllowancePercentage = 20,

                    MedicalAllowancePercentage = 10,

                    TravelAllowancePercentage = 5
                }));
}

[Test]
public void Delete_ShouldDeleteSalaryStructure_WhenValid()
{
    var structure =
        new SalaryStructure
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                structure.Id))
        .Returns(
            structure);

    service.Delete(
        structure.Id);

    repository.Verify(
        x => x.Delete(
            structure),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void Delete_ShouldThrowNotFoundException_WhenSalaryStructureNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (SalaryStructure?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.Delete(
                Guid.NewGuid()));
}

[Test]
public void GetById_ShouldReturnSalaryStructure_WhenFound()
{
    var structure =
        new SalaryStructure
        {
            Id = Guid.NewGuid(),

            Name = "Default",

            BasicPercentage = 40,

            HraPercentage = 20,

            SpecialAllowancePercentage = 20,

            MedicalAllowancePercentage = 10,

            TravelAllowancePercentage = 10
        };

    repository
        .Setup(x =>
            x.GetById(
                structure.Id))
        .Returns(
            structure);

    var result =
        service.GetById(
            structure.Id);

    Assert.That(
        result.Id,
        Is.EqualTo(
            structure.Id));

    Assert.That(
        result.Name,
        Is.EqualTo(
            "Default"));
}

[Test]
public void GetById_ShouldThrowNotFoundException_WhenSalaryStructureNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (SalaryStructure?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GetById(
                Guid.NewGuid()));
}

[Test]
public void GetAll_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAll())
        .Returns(
            new List<SalaryStructure>
            {
                new SalaryStructure
                {
                    Id = Guid.NewGuid(),

                    Name = "Default",

                    BasicPercentage = 40,

                    HraPercentage = 20,

                    SpecialAllowancePercentage = 20,

                    MedicalAllowancePercentage = 10,

                    TravelAllowancePercentage = 10
                }
            });

    var result =
        service.GetAll();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].Name,
        Is.EqualTo(
            "Default"));

    Assert.That(
        result[0].BasicPercentage,
        Is.EqualTo(40));
}


}
