#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProiectP3_BackendApp.Models;
using ProiectP3_BackendApp.Trees;

namespace ProiectP3_BackendApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderContext _context;

        public RBTree<Order> rBTree;

        public OrdersController(OrderContext context)
        {
            _context = context;
            rBTree = new RBTree<Order>();
            var task = FetchOrdersAsync();
            task.Wait();
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<Order>> GetOrders()
        {
            await FetchOrdersAsync();
            var x = rBTree.Minimum(rBTree.Root).val;
            rBTree.Delete(rBTree.Minimum(rBTree.Root).val);
            _context.Remove(x);
            return x;
            //return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            await FetchOrdersAsync();
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            rBTree.Insert(order);

            string jsonString = JsonConvert.SerializeObject(_context.Orders.ToArray());
            if (jsonString != "")
            {
                string fileName = $"{System.IO.Directory.GetCurrentDirectory()}\\json\\orders.json";
                System.IO.File.WriteAllText(fileName, jsonString);
            }

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            rBTree.Delete(order);

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private async Task FetchOrdersAsync()
        {
            string fileName = $"{System.IO.Directory.GetCurrentDirectory()}\\json\\orders.json";
            string jsonString = System.IO.File.ReadAllText(fileName).ToString();
            var x = System.Text.Json.JsonSerializer.Deserialize<Order[]>(jsonString);

            foreach (var order in x)
            {
                if (order != null && !_context.Orders.Contains(order))
                {
                    _context.Orders.Add(order);
                    rBTree.Insert(order);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
