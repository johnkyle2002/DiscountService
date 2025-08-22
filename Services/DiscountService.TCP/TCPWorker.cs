using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscountService.TCP
{
    internal class TCPWorker : IHostedService
    {
        private readonly TCPServer _tCPServer;

        public TCPWorker(TCPServer tCPServer)
        {
            this._tCPServer = tCPServer;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _tCPServer.Start(); 
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
