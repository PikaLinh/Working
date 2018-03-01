function getProductById(id) {
    for (var i = 0; i < lstProducts.length; i++) {
        if (lstProducts[i].ProductId == id) {
            return lstProducts[i];
        }
    }
    return null;
}