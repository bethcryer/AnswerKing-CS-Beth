﻿using Answer.King.Domain.Inventory;

namespace Answer.King.Domain.Repositories;

public interface ITagRepository : IAggregateRepository<Tag>
{
    Task<Tag?> GetOne(string name);
}
