var result = [];
var children = document.getElementById('rg_s').children;
for (var i = 0; i < children.length; i++) {
    var element = children[i];
    if (element.className != 'rg_di rg_bx rg_el ivg-i') continue;
    var span = element.querySelector('span.rg_ilmn');
    var size = span.innerHTML.split('-')[0].split('×');
    var x = parseInt(size[0].replace('&nbsp;', '').trim()), y = parseInt(size[1].replace('&nbsp;', '').trim());
    if (x == y & x >= 300) {
        var href = element.querySelector('a.rg_l').getAttribute('href');
        result.push(href.replace('/imgres?imgurl=', ''));
    }
    if (result.length > 10 || i > 20)
        break;
}

result.join(' ');