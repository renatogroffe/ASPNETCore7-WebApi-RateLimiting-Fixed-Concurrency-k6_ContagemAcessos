import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  duration: '20s',
  vus: 4,
};

export default function () {
  let res = http.get('http://localhost:5020/contador/concurrency');
  check(res, { 'status 200 OK': (r) => r.status === 200 });
  check(res, { 'status 429 Too Many Requests': (r) => r.status === 429 });
  sleep(1);
}