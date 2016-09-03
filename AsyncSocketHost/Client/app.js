var net = require('net');
var unit = function () {
    var port = 8251;
    var host = '127.0.0.1';
    var client = new net.Socket();
    client.setEncoding('utf-8');
    client.connect(port, host, function () {
        var arr = [];
        var time = (new Date()).getTime();
        for (var i = 0; i < 100; i++) {
            arr.push(["m" + i, Math.random(), time]);
            arr.push(["k" + i, Math.random(), time]);
        }
        var json = JSON.stringify({ Command: 'Test', Data: arr });
        var message = (function (str) {
            var ret = str.length.toString();
            var len = ret.length;
            for (var i = 10 - len; i > 0; i--) {
                ret = '0' + ret;
            }
            return ret + str;
        })(json);
        client.write(message);
        message = '';
        var total = 0;
        var count = 0;
        client.on('data', function (data) {
            if (total == 0) {
                if (data.length > 10) {
                    total = parseInt(data.substr(0, 10));
                    if (total > 0) {
                        message += data.substr(10);
                        count += data.length - 10;
                        if (count == total) {
                            //console.log('recv:' + message);
                            client.end();
                        }
                    } else {
                        client.end();
                    }
                }
                else {
                    throw 'invalid data!';
                }
                
            } else {
                if (count + data.length < total) {
                    message += data;
                    count += data.length;
                } else {
                    message += data;
                    // console.log('recv:' + message);
                    client.end();
                }
            }
        });
    });
    client.on('error', function (error) {
        console.dir('error:' + error);
        client.end();
    });
    client.on('close', function () {
        console.log('connection closed');
    });
}
function Loop(count) {
    for (var i = 0; i < count; i++) {
        unit();
    }
}
function Timer(time) {
    var callee = arguments.callee;
    var timeout=setTimeout(function () { Loop(1000); clearTimeout(timeout); callee(time); },time);
}
//console.time('test');
Timer(10000);
//console.timeEnd('test');
//setTimeout(function () { }, 1000000);