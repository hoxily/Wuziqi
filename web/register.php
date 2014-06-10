<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>通行证注册</title>
<link href="css/font.css" rel="stylesheet" type="text/css" />
<style>
body {
    font-family: sans-serif;
}
.hide {
    display: none;
}
.tip {
    color: #888800;
}
.error {
    color: #ff0000;
}
span#checkcode {
    font-family: Superscript, monospace;
    color: #ffffff;
    background-color: #000000;
    font-size: 1.25em;
    margin-left: 0.5em;
}
label {
    float: left;
    width: 80px;
    text-align: right;
    margin-right: 0.5em;
}
</style>
<script type="text/javascript" src="script/jquery-2.1.0.min.js"></script>
<script type="text/javascript">
/*
 * 由于IE11的Number对象不支持parseInt方法，所以需要给它加上
 */
if (!window.Number.parseInt) {
    window.Number.parseInt = window.parseInt;
}

inputFields = {};
function generate_checkcode(upperBound) {
    var a = Math.floor(Math.random() * upperBound);
    var b = Math.floor(Math.random() * upperBound);
    return a + ' + ' + b;
}

function check_checkcode() {
    var str = $('#checkcode').text();
    var splits = str.split('+');
    var a = Number.parseInt(splits[0]);
    var b = Number.parseInt(splits[1]);
    var sum = a + b;
    var input = $('input[type=text][name=checkcode]')[0];
    var str = $(input).val();
    var check = Number.parseInt(str);
    return sum == check;
}

function check_form_valid(form) {
    var ret = true;
    // 提交前全部重新检测一遍
    for (var i in inputFields) {
        inputFields[i].input.blur();
    }
    for (var i in inputFields) {
        if (!inputFields[i].valid) {
            inputFields[i].input.focus();
            return false;
        }
    }
    return ret;
}
function init_input_object(name) {
    o = {};
    o['input'] = $($('input[name=' + name + ']')[0]);
    o['tip'] = $('#' + name + '-tip');
    o['error'] = $('#' + name + '-error');
    o['valid'] = false;
    return o;
}
function init_input_event(obj, pattern_or_checker) {
    obj.input.on('focus', function () {
        obj.tip.removeClass('hide');
        obj.error.addClass('hide');
    });
    obj.input.on('blur', function() {
        obj.tip.addClass('hide');
        if (typeof pattern_or_checker == 'function') {
            obj.valid = pattern_or_checker();
            if (obj.valid) {
                obj.error.addClass('hide');
            }
            else {
                obj.error.removeClass('hide');
            }
        }
        else {
            obj.valid = pattern_or_checker.test(obj.input.val());
            if (obj.valid) {
                obj.error.addClass('hide');
            }
            else {
                obj.error.removeClass('hide');
            }
        }
    });
}
function check_confirm_password() {
    var o = $('input[type=password]');
    var p1 = $(o[0]);
    var p2 = $(o[1]);
    return p1.val() == p2.val();
}
function init_input_fields(obj) {
    obj['password'] = init_input_object('password');
    init_input_event(obj['password'], /^.{6,16}$/);
    
    obj['confirm-password'] = init_input_object('confirm-password');
    init_input_event(obj['confirm-password'], check_confirm_password);

    obj['name'] = init_input_object('name');
    init_input_event(obj['name'], /^.{1,16}$/);

    obj['birthday'] = init_input_object('birthday');
    init_input_event(obj['birthday'], /^[0-9]{4}-[0-9]{2}-[0-9]{2}$/);

    obj['email'] = init_input_object('email');
    init_input_event(obj['email'], /^[^@]+@[^@]+$/);

    obj['qq'] = init_input_object('qq');
    init_input_event(obj['qq'], /^[0-9]+$/);

    obj['mobilephone'] = init_input_object('mobilephone');
    init_input_event(obj['mobilephone'], /^1[0-9]{10}$/);

    obj['signature'] = init_input_object('signature');
    init_input_event(obj['signature'], /^.+$/);

    obj['checkcode'] = init_input_object('checkcode');
    init_input_event(obj['checkcode'], check_checkcode);
}

$(function() {
    // on DOM ready
    init_input_fields(inputFields);
    $('#checkcode').on('click', function() {
        $(this).text(generate_checkcode(50));
        inputFields.checkcode.input.blur();// don't forget trigger blur event
    });
    inputFields.birthday.input.val('2014-06-15');
    inputFields.birthday.valid = true;
    $('#checkcode').text(generate_checkcode(50));
});
</script>
</head>
<body>

<form action="action.php?action=register" method="post" onsubmit="return check_form_valid(this);">
    <p><label>密码</label><input title="长度为6至16个字符的密码" maxlength="16" type="password" name="password" /><span class="hide tip" id="password-tip">长度为6至16个字符</span><span class="hide error" id="password-error">长度为6至16个字符</span></p>
    <p><label>确认密码</label><input title="再输一遍密码以确认没有输错" maxlength="16" type="password" name="confirm-password" /><span class="hide tip" id="confirm-password-tip">请再次输入密码</span><span class="hide error" id="confirm-password-error">密码不一致</span></p>
    <p><label>昵称</label><input title="你自己以及其他玩家看到的称谓" maxlength="16" type="text" name="name" /><span class="hide tip" id="name-tip">请输入昵称</span><span class="hide error" id="name-error">昵称不可以为空</span></p>
    <p><label>生日</label><input title="你的生日" type="date" id="birthday" name="birthday" /><span class="hide tip" id="birthday-tip">你的生日</span><span class="hide error" id="birthday-error">请正确填写年-月-日</span></p>
    <p><label>电子邮箱</label><input title="用于找回密码，很重要" type="email" name="email" maxlength="128" /><span class="hide tip" id="email-tip">用于找回密码的邮箱</span><span class="hide error" id="email-error">请正确填写邮箱</span></p>
    <p><label>QQ号</label><input title="你的腾讯QQ号码" type="text" name="qq" maxlength="16" /><span class="hide tip" id="qq-tip">你的QQ号码</span><span class="hide error" id="qq-error">请正确填写QQ号码</span></p>
    <p><label>手机号</label><input title="11位手机号码" type="text" name="mobilephone" maxlength="11" /><span class="hide tip" id="mobilephone-tip">11位手机号码</span><span class="hide error" id="mobilephone-error">请正确填写手机号码</span></p>
    <p><label>个性签名</label><input title="展示你兴趣爱好的个性签名" type="text" name="signature" maxlength="128" /><span class="hide tip" id="signature-tip">展示你自己的个性签名（128字以内）</span><span class="hide error" id="signature-error">不可以为空</span></p>
    <p><label>验证码</label><input title="计算右侧加法结果并填入" maxlength="2" type="text" name="checkcode" /><span id="checkcode" title="点击切换"></span><span class="hide tip" id="checkcode-tip">计算左侧加法结果并填入以证明不是机器自动注册</span><span class="hide error" id="checkcode-error">结果不正确</span></p>
    <p><input type="submit" value="确认注册" /></p>
</form>
<p>点击注册表明您理解并同意我们的服务条款</p>
</body>
</html>
