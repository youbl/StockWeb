window.onload = function () {
    var vm = new Vue({
        el: '#box',
        data: {
            msg: 'Hello World!'
        },
        methods: {
            get: function () {
                //发送get请求
                this.$http.get('/try/ajax/ajax_info.txt').then(function (res) {
                    document.write(res.body);
                }, function () {
                    console.log('请求失败处理');
                });
            }
        }
    });
}

function doExport() {
    axios.get('/user?ID=12345')
        .then(function (response) {
            console.log(response);
        })
        .catch(function (error) {
            alert(error);
        });
}