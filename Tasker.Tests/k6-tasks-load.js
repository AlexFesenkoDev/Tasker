import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 20, // Кількість одночасних користувачів
  duration: '10s', // Тривалість тесту
};

const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoicXdlcnR5IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTc1MzExMDc2NSwiaXNzIjoiVGFza2VyQXBpIiwiYXVkIjoiVGFza2VyQ2xpZW50In0.U7TicPKa68WGknqP70Ac1avt_vuLCjl280PxS0ZjcfE'; // сюди встав свій JWT токен

export default function () {
  const res = http.get('https://localhost:44341/api/tasks', {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
  sleep(1);
}
