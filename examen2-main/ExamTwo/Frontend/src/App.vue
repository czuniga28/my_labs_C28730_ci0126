<template>
  <div class="machine-container">
    <div class="header">
      <h1>☕ Máquina de Café</h1>
      <p>Seleccione su café y realice su pago</p>
    </div>

    <div class="main-content">
      <!-- Sección de Cafés Disponibles -->
      <div class="section">
        <h2>Cafés Disponibles</h2>
        <div class="coffee-list">
          <div
            v-for="coffee in coffees"
            :key="coffee.name"
            :class="['coffee-item', { disabled: !coffee.isAvailable }]"
          >
            <div class="coffee-info">
              <div class="coffee-name">{{ coffee.name }}</div>
              <div class="coffee-details">
                Precio: ₡{{ formatPrice(coffee.price) }} | Disponible: {{ coffee.quantity }} unidades
              </div>
            </div>
            <div class="coffee-controls">
              <input
                type="number"
                class="quantity-input"
                :min="0"
                :max="coffee.quantity"
                :value="order[coffee.name] || 0"
                :disabled="!coffee.isAvailable"
                @input="updateOrder(coffee.name, $event.target.value)"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Sección de Pago -->
      <div class="section">
        <h2>Pago</h2>
        <div class="payment-section">
          <div class="total-display">
            <div class="total-label">Total a Pagar</div>
            <div class="total-amount">₡{{ formatPrice(totalCost) }}</div>
          </div>

          <div class="total-display" style="background: #e7f3ff; border-color: #2196F3;">
            <div class="total-label">Dinero Ingresado</div>
            <div class="total-amount" style="color: #2196F3;">₡{{ formatPrice(payment.totalAmount) }}</div>
            <div v-if="payment.totalAmount > 0" style="margin-top: 10px; font-size: 0.85em; color: #6c757d;">
              <div v-if="payment.coins.length > 0">
                <strong>Monedas:</strong> {{ formatPaymentBreakdown(payment.coins) }}
              </div>
              <div v-if="payment.bills.length > 0" style="margin-top: 5px;">
                <strong>Billetes:</strong> {{ formatPaymentBreakdown(payment.bills) }}
              </div>
            </div>
          </div>

          <div>
            <h3 style="margin-bottom: 15px; color: #495057;">Monedas</h3>
            <div class="payment-methods">
              <button
                v-for="coin in coins"
                :key="coin"
                class="payment-button"
                @click="addPayment(coin, 'coin')"
              >
                ₡{{ coin }}
              </button>
            </div>
          </div>

          <div>
            <h3 style="margin-bottom: 15px; color: #495057;">Billetes</h3>
            <div class="payment-methods">
              <button
                v-for="bill in bills"
                :key="bill"
                class="payment-button"
                @click="addPayment(bill, 'bill')"
              >
                ₡{{ formatPrice(bill) }}
              </button>
            </div>
          </div>

          <div class="action-buttons">
            <button
              class="btn-primary"
              :disabled="!canPurchase"
              @click="processPurchase"
            >
              Comprar
            </button>
            <button class="btn-secondary" @click="clearAll">Limpiar</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Área de Mensajes y Vuelto -->
    <div v-if="message" :class="['message-area', messageType]">
      <div>
        <strong>{{ message }}</strong>
        <div v-if="changeBreakdown" class="change-breakdown">
          <h3>Su vuelto es de {{ formatPrice(changeAmount) }} colones.</h3>
          <div style="margin-top: 10px;">
            <div
              v-for="(quantity, denomination) in sortedChangeBreakdown"
              :key="denomination"
              class="change-item"
            >
              <span>{{ quantity }} moneda{{ quantity > 1 ? 's' : '' }} de {{ denomination }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { coffeeMachineApi } from './services/api'

export default {
  name: 'App',
  data() {
    return {
      coffees: [],
      order: {},
      payment: {
        totalAmount: 0,
        coins: [],
        bills: []
      },
      totalCost: 0,
      message: '',
      messageType: '',
      changeBreakdown: null,
      changeAmount: 0,
      isLoading: false,
      coins: [25, 50, 100, 500],
      bills: [1000]
    }
  },
  computed: {
    canPurchase() {
      const hasOrder = Object.values(this.order).some(qty => qty > 0)
      const hasPayment = this.payment.totalAmount > 0
      return hasOrder && hasPayment && !this.isLoading
    },
    sortedChangeBreakdown() {
      if (!this.changeBreakdown) return {}
      const sorted = {}
      Object.keys(this.changeBreakdown)
        .map(Number)
        .sort((a, b) => b - a)
        .forEach(key => {
          sorted[key] = this.changeBreakdown[key]
        })
      return sorted
    }
  },
  watch: {
    order: {
      handler() {
        this.calculateTotal()
      },
      deep: true
    }
  },
  mounted() {
    this.loadCoffees()
  },
  methods: {
    formatPrice(price) {
      return new Intl.NumberFormat('es-CR').format(price)
    },
    formatPaymentBreakdown(payments) {
      if (!payments || payments.length === 0) return 'Ninguno'
      
      const counts = {}
      payments.forEach(amount => {
        counts[amount] = (counts[amount] || 0) + 1
      })
      
      const breakdown = Object.entries(counts)
        .map(([amount, count]) => {
          const formattedAmount = this.formatPrice(parseInt(amount))
          return `${count}x ₡${formattedAmount}`
        })
        .join(', ')
      
      return breakdown || 'Ninguno'
    },
    async loadCoffees() {
      try {
        const data = await coffeeMachineApi.getAvailableCoffees()
        this.coffees = data
      } catch (error) {
        this.showMessage('Error al cargar los cafés disponibles', 'error')
      }
    },
    updateOrder(coffeeName, value) {
      const quantity = parseInt(value) || 0
      if (quantity > 0) {
        this.order[coffeeName] = quantity
      } else {
        delete this.order[coffeeName]
      }
      this.calculateTotal()
    },
    async calculateTotal() {
      if (Object.keys(this.order).length === 0) {
        this.totalCost = 0
        return
      }

      try {
        const total = await coffeeMachineApi.calculateTotal(this.order)
        this.totalCost = total
      } catch (error) {
        this.showMessage('Error al calcular el total', 'error')
      }
    },
    addPayment(amount, type) {
      if (type === 'coin') {
        this.payment.coins.push(amount)
      } else {
        this.payment.bills.push(amount)
      }
      this.payment.totalAmount = this.payment.coins.reduce((a, b) => a + b, 0) +
                                  this.payment.bills.reduce((a, b) => a + b, 0)
    },
    async processPurchase() {
      if (!this.canPurchase) return

      this.isLoading = true
      this.message = ''
      this.changeBreakdown = null
      this.changeAmount = 0

      try {
        const result = await coffeeMachineApi.processPurchase(this.order, this.payment)

        if (result.success) {
          this.showMessage(result.message, 'success')
          if (result.change) {
            this.changeBreakdown = result.change.breakdown || {}
            this.changeAmount = result.change.totalAmount || 0
          }
          await this.loadCoffees()
          this.clearAll()
        } else {
          this.showMessage(result.message, 'error')
        }
      } catch (error) {
        let errorMessage = 'Error al procesar la compra'
        if (error.response?.data) {
          if (typeof error.response.data === 'string') {
            errorMessage = error.response.data
          } else if (error.response.data.message) {
            errorMessage = error.response.data.message
          } else if (error.response.data.Message) {
            errorMessage = error.response.data.Message
          }
        } else if (error.message) {
          errorMessage = error.message
        }
        this.showMessage(errorMessage, 'error')
      } finally {
        this.isLoading = false
      }
    },
    clearAll() {
      this.order = {}
      this.payment = {
        totalAmount: 0,
        coins: [],
        bills: []
      }
      this.totalCost = 0
      this.message = ''
      this.changeBreakdown = null
      this.changeAmount = 0
    },
    showMessage(msg, type) {
      this.message = msg
      this.messageType = type
    }
  }
}
</script>

