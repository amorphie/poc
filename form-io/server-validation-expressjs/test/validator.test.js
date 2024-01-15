const request = require("supertest")
const app = require("../src/app")

describe("GET /", () => {
    it("should connect", async () => {
        return request(app)
            .get("")
            .expect('Content-Type', /json/)
            .expect(200)
            .then((res) => {
                expect(res.statusCode).toBe(200);
            })
    });
});