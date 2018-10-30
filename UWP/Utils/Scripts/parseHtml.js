let result = [];
//let children = document.getElementById('rg_s').children;
//for (var i = 0; i < children.length; i++) {
//    let element = children[i];

//    if (element.className !== 'rg_bx rg_di rg_el ivg-i')
//        continue;

    

//    //let x = parseInt(size[0].replace('&nbsp;', '').trim());
//    //let y = parseInt(size[1].replace('&nbsp;', '').trim());

//    //if (x === y & x >= 300) {
//    //    let href = element.getElementsByTagName('a')[0].getAttribute('href');
//    //    result.push(href.replace('/imgres?imgurl=', ''));
//    //}
//    //if (result.length > 10 || i > 20)
//    //    break;
//}
const parseSize = (span) => {
    const size = span.innerHTML.split('-')[0].split('×')

    if (size.length < 2) {
        return null
    }

    const x = size[0].replace('&nbsp;', '').trim()
    const y = size[1].replace('&nbsp;', '').trim()

    return { width: x, height: y }
}

const divs = document.getElementsByClassName('rg_bx rg_di rg_el ivg-i')

for (let i = 0; i < divs.length; i++) {
    const el = divs[i]

    const span = el.getElementsByTagName('span')[0]
    const size = parseSize(span)

    // Cover size must be at least 500 pixels
    if (size !== null && size.width === size.height && size.width >= 500) {
        const elements = el.getElementsByTagName('a')
        if (elements.length > 0) {
            let href = elements[0].getAttribute('href').replace('/imgres?imgurl=', '').split('&')
            if (href.length > 0) {
                result.push(decodeURIComponent(href[0]))
            }
        }
    }

}


result.join('\n')
