<template>
  <view class="wrap">
    <!-- 1. 吸顶 tab -->
    <u-sticky offset-top="0">
      <u-tabs
        :list="list"
        :current="current"
        lineColor="#e45656"
        lineWidth="40"
        :activeStyle="{ color: '#e45656' }"
        :inactiveStyle="{ color: '#606266' }"
        @change="tabsChange"
      />
    </u-sticky>

    <!-- 2. swiper 内容区 -->
    <swiper class="swiper-box" :current="current" @change="swiperChange">
      <swiper-item v-for="(tabItem, tabIndex) in list" :key="tabIndex">
        <scroll-view scroll-y class="scroll-area" @scrolltolower="reachBottom">
          <!-- 空状态：垂直水平居中 -->
          <view class="empty-box" v-if="!orderList[tabIndex] || !orderList[tabIndex].length">
            <u-empty
              mode="order"
              text="您还没有相关订单"
            >
              <u-button slot="bottom" type="error" plain size="mini" @click="goAround">
                随便逛逛
              </u-button>
            </u-empty>
          </view>

          <!-- 订单列表（已移除滑动删除） -->
          <view v-else>
            <view
              v-for="res in orderList[tabIndex]"
              :key="res.id"
              class="order-card"
            >
              <!-- 店铺行 -->
              <view class="card-head">
                <u-icon name="home" size="28" color="info" />
                <text class="store">{{ res.store }}</text>
                <u-icon name="arrow-right" size="22" color="#c0c4cc" />
                <u-tag :text="res.deal" type="error" size="mini" plain />
              </view>

              <!-- 商品行 -->
              <view
                v-for="(item, i) in res.goodsList"
                :key="i"
                class="goods"
                @click="toDetail"
              >
                <u-image width="200rpx" height="200rpx" :src="item.goodsUrl" radius="12" />
                <view class="info">
                  <u-text :text="item.title" lines="2" size="28rpx" bold />
                  <u-text :text="item.type" color="#909399" size="24rpx" margin="4rpx 0" />
                  <u-text :text="`发货时间 ${item.deliveryTime}`" color="#c0c4cc" size="24rpx" />
                </view>

                <view class="price-col">
                  <text class="price">￥{{ item.price }}</text>
                  <text class="count">x{{ item.number }}</text>
                </view>
              </view>

              <view class="total-line">
                共{{ totalNum(res.goodsList) }}件商品 合计：
                <text class="total-price">￥{{ totalPrice(res.goodsList) }}</text>
              </view>

              <view class="actions">
                <u-button plain size="mini" shape="circle" color="#606266" @click="logistics">
                  查看物流
                </u-button>
                <u-button plain size="mini" shape="circle" color="#606266" @click="exchange">
                  卖了换钱
                </u-button>
                <u-button plain color="#e45656" size="mini" shape="circle" @click="evaluate">
                  评价
                </u-button>
              </view>
            </view>

            <u-loadmore :status="loadStatus[tabIndex]" margin-top="30" />
          </view>
        </scroll-view>
      </swiper-item>
    </swiper>
  </view>
</template>

<script>
export default {
  data() {
    return {
      orderList: [[], [], [], []],
      dataList: [
        {
          id: 1,
          store: '夏日流星限定贩卖',
          deal: '交易成功',
          goodsList: [
            {
              goodsUrl: '//img13.360buyimg.com/n7/jfs/t1/103005/7/17719/314825/5e8c19faEb7eed50d/5b81ae4b2f7f3bb7.jpg',
              title: '【冬日限定】现货 原创jk制服女2020冬装新款小清新宽松软糯毛衣外套女开衫短款百搭日系甜美风',
              type: '灰色;M',
              deliveryTime: '付款后30天内发货',
              price: '348.58',
              number: 2
            },
            {
              goodsUrl: '//img12.360buyimg.com/n7/jfs/t1/102191/19/9072/330688/5e0af7cfE17698872/c91c00d713bf729a.jpg',
              title: '【葡萄藤】现货 小清新学院风制服格裙百褶裙女短款百搭日系甜美风原创jk制服女2020新款',
              type: '45cm;S',
              deliveryTime: '付款后30天内发货',
              price: '135.00',
              number: 1
            }
          ]
        },
        {
          id: 2,
          store: '江南皮革厂',
          deal: '交易失败',
          goodsList: [
            {
              goodsUrl: '//img14.360buyimg.com/n7/jfs/t1/60319/15/6105/406802/5d43f68aE9f00db8c/0affb7ac46c345e2.jpg',
              title: '【冬日限定】现货 原创jk制服女2020冬装新款小清新宽松软糯毛衣外套女开衫短款百搭日系甜美风',
              type: '粉色;M',
              deliveryTime: '付款后7天内发货',
              price: '128.05',
              number: 1
            }
          ]
        },
        {
          id: 3,
          store: '三星旗舰店',
          deal: '交易失败',
          goodsList: [
            {
              goodsUrl: '//img11.360buyimg.com/n7/jfs/t1/94448/29/2734/524808/5dd4cc16E990dfb6b/59c256f85a8c3757.jpg',
              title: '三星（SAMSUNG）京品家电 UA65RUF70AJXXZ 65英寸4K超高清 HDR 京东微联 智能语音 教育资源液晶电视机',
              type: '4K，广色域',
              deliveryTime: '保质5年',
              price: '1998',
              number: 3
            },
            {
              goodsUrl: '//img14.360buyimg.com/n7/jfs/t6007/205/4099529191/294869/ae4e6d4f/595dcf19Ndce3227d.jpg!q90.jpg',
              title: '美的(Midea)639升 对开门冰箱 19分钟急速净味 一级能效冷藏双开门杀菌智能家用双变频节能 BCD-639WKPZM(E)',
              type: '容量大，速冻',
              deliveryTime: '保质5年',
              price: '2354',
              number: 1
            }
          ]
        },
        {
          id: 4,
          store: '三星旗舰店',
          deal: '交易失败',
          goodsList: [
            {
              goodsUrl: '//img10.360buyimg.com/n7/jfs/t22300/31/1505958241/171936/9e201a89/5b2b12ffNe6dbb594.jpg!q90.jpg',
              title: '法国进口红酒 拉菲（LAFITE）传奇波尔多干红葡萄酒750ml*6整箱装',
              type: '4K，广色域',
              deliveryTime: '珍藏10年好酒',
              price: '1543',
              number: 3
            },
            {
              goodsUrl: '//img10.360buyimg.com/n7/jfs/t1/107598/17/3766/525060/5e143aacE9a94d43c/03573ae60b8bf0ee.jpg',
              title: '蓝妹（BLUE GIRL）酷爽啤酒 清啤 原装进口啤酒 罐装 500ml*9听 整箱装',
              type: '一打',
              deliveryTime: '口感好',
              price: '120',
              number: 1
            }
          ]
        },
        {
          id: 5,
          store: '三星旗舰店',
          deal: '交易成功',
          goodsList: [
            {
              goodsUrl: '//img12.360buyimg.com/n7/jfs/t1/52408/35/3554/78293/5d12e9cfEfd118ba1/ba5995e62cbd747f.jpg!q90.jpg',
              title: '企业微信 中控人脸指纹识别考勤机刷脸机 无线签到异地多店打卡机WX108',
              type: '识别效率高',
              deliveryTime: '使用方便',
              price: '451',
              number: 9
            }
          ]
        }
      ],
      list: [
        { name: '待付款' },
        { name: '待发货' },
        { name: '待收货' },
        { name: '待评价', count: 12 }
      ],
      current: 0,
      loadStatus: ['loadmore', 'loadmore', 'loadmore', 'loadmore']
    };
  },
  onLoad() {
    [0, 1, 2, 3].forEach(i => this.getOrderList(i));
  },
  methods: {
    tabsChange(e) {
      this.current = e.index;
      this.getOrderList(e.index);
    },
    swiperChange(e) {
      this.current = e.detail.current;
    },
    reachBottom() {
      this.$set(this.loadStatus, this.current, 'loading');
      setTimeout(() => {
        this.getOrderList(this.current);
        this.$set(this.loadStatus, this.current, 'loadmore');
      }, 1200);
    },
    getOrderList(idx) {
      for (let i = 0; i < 5; i++) {
        const index = this.$u.random(0, this.dataList.length - 1);
        const data = JSON.parse(JSON.stringify(this.dataList[index]));
        data.id = this.$u.guid();
        this.orderList[idx].push(data);
      }
    },
    totalPrice(list) {
      return list.reduce((s, v) => s + parseFloat(v.price) * v.number, 0).toFixed(2);
    },
    totalNum(list) {
      return list.reduce((s, v) => s + v.number, 0);
    },
    goAround() { uni.switchTab({ url: '/pages/index/index' }); },
    logistics() { uni.showToast({ title: '查看物流', icon: 'none' }); },
    exchange() { uni.showToast({ title: '卖了换钱', icon: 'none' }); },
    evaluate() { uni.showToast({ title: '评价', icon: 'none' }); },
    toDetail() { uni.showToast({ title: '进入详情', icon: 'none' }); }
  }
};
</script>

<style lang="scss" scoped>
.wrap {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: #faf8f5;
}

.scroll-area {
  height: 100%;
}
.swiper-box {
  flex: 1;
}

/* 空状态垂直水平居中 */
.empty-box {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
}

.order-card {
  margin: 24rpx;
  background: #ffffff;
  border-radius: 24rpx;
  overflow: hidden;
  box-shadow: 0 4rpx 20rpx rgba(0, 0, 0, 0.05);
}

.card-head {
  display: flex;
  align-items: center;
  padding: 32rpx 32rpx 24rpx;
  border-bottom: 1rpx solid #f0f0f0;
  .store {
    margin: 0 12rpx;
    font-size: 32rpx;
    font-weight: 600;
    color: #2c2c2c;
  }
}

.goods {
  display: flex;
  padding: 32rpx;
  border-bottom: 1rpx solid #f0f0f0;
  .info {
    flex: 1;
    margin: 0 24rpx;
  }
  .price-col {
    text-align: right;
    align-self: flex-end;
    .price {
      font-size: 32rpx;
      font-weight: 600;
      color: #ff675f;
    }
    .count {
      margin-top: 4rpx;
      font-size: 24rpx;
      color: #999999;
    }
  }
}

.total-line {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding: 24rpx 32rpx;
  font-size: 28rpx;
  color: #666666;
  .total-price {
    margin-left: 8rpx;
    color: #ff675f;
    font-weight: 600;
  }
}

.actions {
  display: flex;
  justify-content: flex-end;
  gap: 24rpx;
  padding: 0 32rpx 32rpx;
}
</style>