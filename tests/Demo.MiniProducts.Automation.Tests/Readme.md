## Automation Tests

* Install HTTP library from Nuget.
* Tests are written in `AAA` (Arrange, act, assert) pattern.

### Register Product Tests

* Invalid request
  * Must return 400 HTTP status code.
  * Must return header `content-type` with a value `problem-details/json`.
  * Must return response body containing the fields which have errors.

* Valid request
  * 201 HTTP status code is expected.
  * Response header must contain a location to retrieve the product.


 