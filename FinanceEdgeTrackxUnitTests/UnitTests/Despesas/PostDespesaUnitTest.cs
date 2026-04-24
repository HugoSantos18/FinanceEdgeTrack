using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Despesas;

public class PostDespesaUnitTest
{
    private readonly DespesaUnitTestController _helper;

    public PostDespesaUnitTest()
    {
        _helper = new DespesaUnitTestController();
    }

    private CreateDespesaDTO CreateSeed(int index)
    {
        var despesas = new List<CreateDespesaDTO>
        {
            new() {Titulo = "Despesa1", Descricao = "Despesa1", Data = DateTime.UtcNow, Fixa = false, Valor = 150},
            new() {Titulo = "Despesa2", Descricao = "Despesa2", Data = DateTime.UtcNow, Fixa = true, Valor = 300},
            new() {Titulo = "Despesa3", Descricao = "Despesa3", Data = DateTime.UtcNow, Fixa = true, Valor = -10},
            new() {Titulo = "", Descricao = "Despesa3", Data = DateTime.UtcNow, Fixa = false, Valor = 100},
        };

        if (index < 0 || index > despesas.Count)
            throw new ArgumentOutOfRangeException("index");

        return despesas[index];
    }

    [Fact]
    public async Task PostDespesa_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        var despesaDto = CreateSeed(0);
        _helper.serviceMock
            .Setup(s => s.CreateDespesaAsync(despesaDto))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Ok(_helper.mapper.Map<DespesaDTO>(despesaDto)));

        // act
        var result = await _helper.controller.Post(despesaDto);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(despesaDto.Valor, response.Data!.Valor);
        _helper.serviceMock.Verify(s => s.CreateDespesaAsync(despesaDto), Times.Once);
    }

    [Fact]
    public async Task PostDespesa_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        var invalidDespesaDto = CreateSeed(3);
        _helper.serviceMock
            .Setup(s => s.CreateDespesaAsync(invalidDespesaDto))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Fail("Não foi possível criar a despesa."));

        // act
        var result = await _helper.controller.Post(invalidDespesaDto);

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(bad.Value);
        Assert.False(response.Success);
        Assert.True(invalidDespesaDto.Valor > 0);
        Assert.True(invalidDespesaDto.Titulo.IsNullOrEmpty());
        _helper.serviceMock.Verify(s => s.CreateDespesaAsync(invalidDespesaDto));
    }


    [Fact]
    public async Task PostDespesaFixa_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        var despesaDto = CreateSeed(0);
        _helper.serviceMock
            .Setup(s => s.CreateDespesaAsync(despesaDto))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Ok(_helper.mapper.Map<DespesaDTO>(despesaDto)));

        // act
        var result = await _helper.controller.Post(despesaDto);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(despesaDto.Fixa, response.Data!.Fixa);
        Assert.Equal(despesaDto.Valor, response.Data!.Valor);
        _helper.serviceMock.Verify(s => s.CreateDespesaAsync(despesaDto), Times.Once);
    }

    [Fact]
    public async Task PostDespesaFixa_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        var invalidDespesaDto = CreateSeed(2);
        _helper.serviceMock
            .Setup(s => s.CreateDespesaAsync(invalidDespesaDto))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Fail("Não foi possível criar a despesa."));

        // act
        var result = await _helper.controller.Post(invalidDespesaDto);

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(bad.Value);
        Assert.False(response.Success);
        Assert.False(invalidDespesaDto.Valor > 0);
        Assert.True(invalidDespesaDto.Fixa = true);
        _helper.serviceMock.Verify(s => s.CreateDespesaAsync(invalidDespesaDto));
    }
}
