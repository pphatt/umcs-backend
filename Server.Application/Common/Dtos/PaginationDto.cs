﻿namespace Server.Application.Common.Dtos;

public class PaginationDto
{
    public string? Keyword { get; set; }

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
