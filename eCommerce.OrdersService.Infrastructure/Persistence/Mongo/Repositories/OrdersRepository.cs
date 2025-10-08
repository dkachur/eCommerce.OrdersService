using AutoMapper;
using eCommerce.OrdersService.Application.RepositoryContracts;
using eCommerce.OrdersService.Domain.Entities;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Config;
using eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eCommerce.OrdersService.Infrastructure.Persistence.Mongo.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly IMongoCollection<OrderDocument> _collection;
    private readonly IMapper _mapper;

    public OrdersRepository(IMongoDatabase database, IOptions<MongoOptions> options, IMapper mapper)
    {
        var collectionName = options.Value.Collections.Orders
            ?? throw new ArgumentException("Orders collection name is not configured");

        _collection = database.GetCollection<OrderDocument>(collectionName);
        _mapper = mapper;
    }

    public async Task<Order?> AddOrderAsync(Order order, CancellationToken ct = default)
    {
        var doc = _mapper.Map<OrderDocument>(order);

        try
        {
            await _collection.InsertOneAsync(doc, cancellationToken: ct);
        }
        catch (MongoWriteException)
        {
            return null;
        }

        var saved = _mapper.Map<Order>(doc);
        return saved;
    }

    public async Task<bool> DeleteOrderAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(d => d.OrderId, id);
        var delRes = await _collection.DeleteOneAsync(filter, ct);
        return delRes.DeletedCount > 0;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(o => o.OrderId, id);
        var doc = await _collection.Find(filter).FirstOrDefaultAsync(ct);
        return doc is null ? null : _mapper.Map<Order>(doc);
    }

    public async Task<List<Order>> GetOrdersAsync(CancellationToken ct = default)
    {
        var docs = await _collection.Find(Builders<OrderDocument>.Filter.Empty).ToListAsync(ct);
        return _mapper.Map<List<Order>>(docs);
    }

    public async Task<List<Order>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        var filter = Builders<OrderDocument>.Filter.ElemMatch(
            d => d.OrderItems, 
            Builders<OrderItemDocument>.Filter.Eq(i => i.ProductId, productId)
        );

        var docs = await _collection.Find(filter).ToListAsync(ct);
        return _mapper.Map<List<Order>>(docs);
    }

    public async Task<List<Order>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(d => d.UserId, userId);

        var docs = await _collection.Find(filter).ToListAsync(ct);
        return _mapper.Map<List<Order>>(docs);
    }

    public async Task<List<Order>> GetByOrderDate(DateTime orderDate, CancellationToken ct = default)
    {
        var start = orderDate.Date;
        var end = start.AddDays(1).AddTicks(-1);

        var filter = Builders<OrderDocument>.Filter.And(
            Builders<OrderDocument>.Filter.Gte(d => d.OrderDate, start), 
            Builders<OrderDocument>.Filter.Lte(d => d.OrderDate, end)
        );

        var docs = await _collection.Find(filter).ToListAsync(ct);
        return _mapper.Map<List<Order>>(docs);
    }

    public async Task<Order?> UpdateOrderAsync(Order order, CancellationToken ct = default)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(f => f.OrderId, order.OrderId);
        var update = Builders<OrderDocument>.Update
            .Set(d => d.UserId, order.UserId)
            .Set(d => d.OrderDate, order.OrderDate)
            .Set(d => d.TotalBill, order.TotalBill)
            .Set(d => d.OrderItems, _mapper.Map<List<OrderItemDocument>>(order.OrderItems));

        var options = new FindOneAndUpdateOptions<OrderDocument>()
        {
            ReturnDocument = ReturnDocument.After,
            IsUpsert = false
        };

        var updatedDoc = await _collection.FindOneAndUpdateAsync(filter, update, options, ct);
        return updatedDoc is null ? null : _mapper.Map<Order>(updatedDoc);
    }
}
