import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5059/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

export const coffeeMachineApi = {
  async getAvailableCoffees() {
    const response = await api.get('/CoffeeMachine/coffees')
    return response.data
  },

  async calculateTotal(order) {
    const response = await api.post('/CoffeeMachine/calculate-total', order)
    return response.data
  },

  async processPurchase(order, payment) {
    const response = await api.post('/CoffeeMachine/purchase', {
      order,
      payment: {
        totalAmount: payment.totalAmount,
        coins: payment.coins,
        bills: payment.bills
      }
    })
    return response.data
  }
}

export default api

