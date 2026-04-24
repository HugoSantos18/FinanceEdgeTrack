using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Receitas;

public class PostReceitaUnitTest
{
    private readonly ReceitaUnitTestController _helper;

    public PostReceitaUnitTest()
    {
        _helper = new ReceitaUnitTestController();
    }

    private CreateReceitaDTO CreateSeed(int index)
    {
        var receitas = new List<CreateReceitaDTO>
        {
            new() {Titulo = "Receita1", Descricao = "Descrição1", Valor = 1000, Data = DateTime.UtcNow }, 
            new() {Titulo = "", Descricao = "Descrição3", Valor = 200, Data = DateTime.UtcNow }, 
            new() {Titulo = "Receita4", Descricao = "Descrição4", Valor = -100, Data = DateTime.UtcNow },
        };

        if (index < 0 || index > receitas.Count)
            throw new ArgumentOutOfRangeException("index");

        return receitas[index];
    }

    [Fact]
    public async Task PostReceita_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        var receitaDto = CreateSeed(0);
        _helper.serviceMock
            .Setup(s => s.CreateReceitaAsync(receitaDto))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Ok(_helper.mapper.Map<ReceitaDTO>(receitaDto)));

        // act
        var result = await _helper.controller.Post(receitaDto);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.True(receitaDto.Valor > 0);
        Assert.Equal(receitaDto.Valor, response.Data!.Valor);
        _helper.serviceMock.Verify(s => s.CreateReceitaAsync(receitaDto), Times.Once);
    }

    [Fact]
    public async Task PostReceita_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        var receitaDto = CreateSeed(2);
        _helper.serviceMock
            .Setup(s => s.CreateReceitaAsync(receitaDto))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Fail("Não foi possível criar a receita."));

        // act
        var result = await _helper.controller.Post(receitaDto);

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(bad.Value);
        Assert.False(response.Success);
        Assert.False(receitaDto.Valor > 0);
        _helper.serviceMock.Verify(s => s.CreateReceitaAsync(receitaDto), Times.Once);
    }

    [Fact]
    public async Task PostInvalidTittleReceita_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        var receitaDto = CreateSeed(1);
        _helper.serviceMock
            .Setup(s => s.CreateReceitaAsync(receitaDto))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Fail("Não foi possível criar a receita."));

        // act
        var result = await _helper.controller.Post(receitaDto);

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(bad.Value);
        Assert.False(response.Success);
        Assert.True(receitaDto.Titulo.IsNullOrEmpty());
        _helper.serviceMock.Verify(s => s.CreateReceitaAsync(receitaDto), Times.Once);
    }

}
