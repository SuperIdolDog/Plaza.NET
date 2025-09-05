<template>
  <view class="page">
    <!-- 顶部导航 -->
    <u-navbar
      title="收货地址"
      leftIcon="arrow-left"
      :border="false"
      :fixed="true"
      bgColor="#fff"
      titleStyle="font-size:34rpx;font-weight:600;color:#262626"
      leftIconColor="#262626"
      @leftClick="() => uni.navigateBack()"
    />

    <!-- 空状态 -->
    <u-empty
      v-if="!siteList.length"
      mode="address"
      marginTop="200rpx"
      icon="https://img10.360buyimg.com/img/jfs/t1/155579/17/42589/5577/65f7ea8eF4a7c5bd6/9c48b8b2b2c9a8b5.png"
      text="您还没有添加收货地址"
      subText="添加地址后，下单更便捷"
    >
      <u-button type="primary" shape="circle" text="新建地址" @click="toAdd" />
    </u-empty>

    <!-- 地址列表 -->
    <scroll-view
      v-else
      scroll-y
      class="list-wrapper"
      :style="{ height: scrollHeight }"
    >
      <u-swipe-action>
        <u-swipe-action-item
          v-for="addr in siteList"
          :key="addr.id"
          :options="delOptions"
          @click="delAddress(addr.id)"
        >
          <view
            class="address-card-v2"
            :class="{ 'address-card-v2--default': addr.isDefault }"
            @click="toEdit(addr)"
          >
            <view class="address-card-v2__left">
              <view v-if="addr.isDefault" class="address-card-v2__default-dot" />
              <u-icon
                :name="addr.isDefault ? 'map-fill' : 'map'"
                :color="addr.isDefault ? '#f23030' : '#c8c9cc'"
                size="48rpx"
              />
            </view>

            <view class="address-card-v2__center">
              <view class="address-card-v2__line1">
                <text class="name">{{ addr.name }}</text>
                <text class="phone">{{ addr.phone }}</text>
                <u-tag
                  v-if="addr.isDefault"
                  text="默认"
                  size="mini"
                  type="error"
                  plain
                />
              </view>
              <view class="address-card-v2__line2">
                {{ addr.province }}{{ addr.city }}{{ addr.county }}{{ addr.detail }}
              </view>
              <view v-if="addr.label" class="address-card-v2__line3">
                <text class="label-tag">{{ addr.label }}</text>
              </view>
            </view>

            <u-icon name="arrow-right" color="#c8c9cc" size="28rpx" />
          </view>
        </u-swipe-action-item>
      </u-swipe-action>
    </scroll-view>

    <!-- 底部按钮 -->
    <view class="footer-btn">
      <u-button type="primary" shape="circle" text="+ 新建收货地址" @click="toAdd" />
    </view>

    <!-- 地址弹窗（使用 v-if 保证 u-form 渲染） -->
    <u-popup
      v-if="showPop"
      :show="showPop"
      mode="bottom"
      round="32rpx 32rpx 0 0"
      :safe-area-inset-bottom="true"
      @close="showPop = false"
    >
      <view class="pop-adaptive">
        <view class="pop-adaptive__header">
          <text>{{ isEdit ? '编辑收货地址' : '新建收货地址' }}</text>
          <u-icon name="close" size="44rpx" @click="showPop = false" />
        </view>

        <scroll-view class="pop-adaptive__body" scroll-y>
          <!-- 关键：ref="form" 与脚本保持一致 -->
          <u-form ref="form" :model="form" label-width="auto" class="form-inner">
            <u-form-item label="收货人" prop="name" border-bottom>
              <u-input v-model="form.name" placeholder="请填写收货人姓名" />
            </u-form-item>

            <u-form-item label="手机号码" prop="phone" border-bottom>
              <u-input v-model="form.phone" type="number" placeholder="请填写手机号码" />
            </u-form-item>

            <u-form-item label="所在地区" prop="region" border-bottom @tap="showRegion = true">
              <u-input
                v-model="form.region"
                placeholder="省市区县、乡镇等"
                suffixIcon="map-fill"
              />
            </u-form-item>

            <u-form-item label="详细地址" prop="detail" border-bottom>
              <u-textarea
                v-model="form.detail"
                :maxlength="50"
                height="160"
                placeholder="街道、门牌号等"
              />
            </u-form-item>

            <u-form-item label="地址标签">
              <view class="tag-list">
                <view
                  v-for="l in labels"
                  :key="l"
                  class="tag"
                  :class="{ active: form.label === l }"
                  @click="form.label = l"
                >
                  {{ l }}
                </view>
              </view>
            </u-form-item>

            <u-form-item>
              <view class="row-center">
                <text>设为默认地址</text>
                <u-switch v-model="form.isDefault" active-color="#f23030" />
              </view>
            </u-form-item>
          </u-form>
        </scroll-view>

        <view class="pop-adaptive__footer">
          <u-button type="primary" shape="round" text="保存" @click="save" />
        </view>
      </view>
    </u-popup>

    <!-- 省市区联动 picker -->
    <u-picker
      :show="showRegion"
      :columns="regionColumns"
      :defaultIndex="regionDefaultIndex"
      @confirm="onRegionConfirm"
      @change="onRegionChange"
      @cancel="showRegion = false"
    />
  </view>
</template>

<script>
import areaData from '@/api/areaData.js';

export default {
  data() {
    return {
      siteList: [],
      labels: ['家', '公司', '学校'],
      showPop: false,
      showRegion: false,
      isEdit: false,
      editId: null,
      form: {
        name: '',
        phone: '',
        province: '',
        city: '',
        county: '',
        detail: '',
        label: '家',
        isDefault: false
      },
      rules: {
        name: { required: true, message: '请输入收货人', trigger: ['blur', 'change'] },
        phone: [
          { required: true, message: '请输入手机号', trigger: ['blur', 'change'] },
          { pattern: /^1[3-9]\d{9}$/, message: '手机号格式不正确', trigger: ['blur', 'change'] }
        ],
        region: { required: true, message: '请选择地区', trigger: 'change' },
        detail: { required: true, message: '请输入详细地址', trigger: ['blur', 'change'] }
      },
      delOptions: [{ text: '删除', style: { backgroundColor: '#f23030' } }],
      regionColumns: [[], [], []],
      regionData: areaData,
      regionDefaultIndex: [0, 0, 0]
    };
  },
  computed: {
    scrollHeight() {
      return 'calc(100vh - 88rpx - 200rpx)';
    }
  },
  onShow() {
    this.getList();
  },
  mounted() {
    this.initRegionPicker();
  },
  methods: {
	 
    /* 获取地址列表 */
    getList() {
		const cached = uni.getStorageSync('userInfo') || {};
      const userId = cached.id;
	  console.log(userId)
      if (!userId) return;
      uni.request({
        url: '/api/User/addressinfo',
        method: 'GET',
        data: { userid: userId },
        success: (res) => {
          this.siteList = res.data || [];
        },
        fail: () => {
          uni.showToast({ title: '获取地址失败', icon: 'none' });
        }
      });
    },

    /* 新增 */
    toAdd() {
      this.isEdit = false;
      this.resetForm();
      this.showPop = true;
      // 等 DOM 渲染完再绑定校验规则
      this.$nextTick(() => this.$refs.form?.setRules(this.rules));
    },

    /* 编辑 */
    toEdit(item) {
		console.log('编辑的地址数据：', item);
      this.isEdit = true;
      this.editId = item.id;
	  const regionStr = `${item.province || ''} ${item.city || ''} ${item.county || ''}`.trim();
      Object.assign(this.form, {
			name: item.name,
           phone: item.phone,
           province: item.province,
           city: item.city,
           county: item.county,
           detail: item.detail,            
           label: item.label,
           isDefault: item.isDefault,
		   region: regionStr
      });
	
      this.echoRegion();
      this.showPop = true;
      this.$nextTick(() => this.$refs.form?.setRules(this.rules));
    },

    /* 保存（新增/编辑） */
    save() {
      this.$refs.form.validate().then(() => {
        const userId = uni.getStorageSync('userInfo').id;
         const payload = {
               userId,
               name: this.form.name,
               phone: this.form.phone,
               province: this.form.province,
               city: this.form.city,
               county: this.form.county,
               detail: this.form.detail,
               label: this.form.label,
               isDefault: this.form.isDefault
             };
			console.log(payload);

        
        uni.request({
          url : this.isEdit ? `/api/User/updateaddress/${this.editId}` : '/api/User/addaddress',
          method: 'POST',
          data: payload,
          success: () => {
            this.showPop = false;
            this.getList();
          },
          fail: () => {
            uni.showToast({ title: '保存失败', icon: 'none' });
          }
        });
      }).catch(() => {});
    },

    /* 删除地址 */
   delAddress(id) {
     uni.showModal({
       title: '提示',
       content: '确认删除该地址？',
       success: ({ confirm }) => {
         if (confirm) {
           uni.request({
             url: `/api/User/deladdress/${id}`,   // 只把 id 放在路径
             method: 'POST',
             data: { isDeleted: true },
             success: () => {
               uni.showToast({ title: '删除成功', icon: 'success' });
               this.getList();
             },
             fail: () => {
               uni.showToast({ title: '删除失败', icon: 'none' });
             }
           });
         }
       }
     });
   },

    /* 初始化省市区列 */
    initRegionPicker() {
      const provinces = this.regionData.map(p => p.name);
      const cities = this.regionData[0].city.map(c => c.name);
      const districts = this.regionData[0].city[0].districtAndCounty || [];
      this.regionColumns = [provinces, cities, districts];
      this.regionDefaultIndex = [0, 0, 0];
    },

    /* 省市区联动 */
    onRegionChange(e) {
      const { columnIndex, index } = e;
      this.$nextTick(() => {
        if (columnIndex === 0) {
          const cities = this.regionData[index].city.map(c => c.name);
          const districts = this.regionData[index].city[0].districtAndCounty || [];
          this.regionColumns = [this.regionColumns[0], cities, districts];
          this.regionDefaultIndex = [index, 0, 0];
        } else if (columnIndex === 1) {
          const provinceIndex = this.regionDefaultIndex[0];
          const districts = this.regionData[provinceIndex].city[index].districtAndCounty || [];
          this.regionColumns = [this.regionColumns[0], this.regionColumns[1], districts];
          this.regionDefaultIndex = [provinceIndex, index, 0];
        }
      });
    },

    /* 选择完成 */
    onRegionConfirm(e) {
      const [province, city, county] = e.value;
      this.form.region = `${province} ${city} ${county}`;
      this.form.province = province;
      this.form.city = city;
      this.form.county = county;
      this.showRegion = false;
    },

    /* 回显默认地址 */
    echoRegion() {
      const { province, city, county } = this.form;
      const pIdx = this.regionData.findIndex(p => p.name === province);
      if (pIdx === -1) return;

      const cIdx = this.regionData[pIdx].city.findIndex(c => c.name === city);
      const dIdx = this.regionData[pIdx].city[cIdx]?.districtAndCounty.indexOf(county) ?? 0;

      const cities = this.regionData[pIdx].city.map(c => c.name);
      const districts = this.regionData[pIdx].city[cIdx].districtAndCounty || [];
      this.regionColumns = [this.regionColumns[0], cities, districts];
      this.regionDefaultIndex = [pIdx, cIdx, dIdx];
    },

    /* 重置表单 */
    resetForm() {
      Object.assign(this.form, {
        name: '',
        phone: '',
        region: '',
        province: '',
        city: '',
        county: '',
        detail: '',
        label: '家',
        isDefault: false
      });
    }
  }
};
</script>

<style lang="scss" scoped>
page { background: #f3f5f7; }
.page { display: flex; flex-direction: column; height: 100vh; }
.list-wrapper { flex: 1; padding: 0 24rpx; }

.address-card-v2 {
  display: flex;
  align-items: center;
  margin: 0 24rpx 24rpx;
  padding: 32rpx;
  background: #fff;
  border-radius: 20rpx;
  box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.05);
  &__left { display: flex; align-items: center; margin-right: 24rpx; }
  &__default-dot { width: 8rpx; height: 64rpx; background: #f23030; border-radius: 0 4rpx 4rpx 0; margin-right: 20rpx; }
  &__center { flex: 1; }
  &__line1 {
    display: flex; align-items: center; font-size: 32rpx; font-weight: 600; margin-bottom: 8rpx;
    .name { margin-right: 20rpx; }
    .phone { margin-right: 20rpx; color: #666; }
  }
  &__line2 { font-size: 28rpx; color: #666; line-height: 1.5; }
  &__line3 { margin-top: 8rpx; }
  .label-tag {
    display: inline-block; padding: 4rpx 12rpx; font-size: 22rpx; color: #f23030;
    background: rgba(#f23030, 0.1); border-radius: 6rpx;
  }
}

.footer-btn {
  position: fixed; left: 0; right: 0; bottom: 0;
  padding: 20rpx 32rpx calc(20rpx + env(safe-area-inset-bottom));
  background: #fff; border-top: 1rpx solid #e8e8e8;
}

.pop-adaptive {
  display: flex; flex-direction: column; max-height: 75vh;
  background: #fff; border-radius: 32rpx 32rpx 0 0; overflow: hidden;
  &__header {
    flex-shrink: 0; display: flex; justify-content: space-between; align-items: center;
    height: 112rpx; padding: 0 32rpx; font-size: 34rpx; font-weight: 600;
    border-bottom: 1rpx solid #f0f0f0;
  }
  &__body { flex: 1; overflow-y: scroll; }
  &__footer {
    flex-shrink: 0; padding: 24rpx 32rpx calc(24rpx + env(safe-area-inset-bottom));
    background: #fff;
  }
}

.form-inner { padding: 0 32rpx; }

.tag-list {
  display: flex; gap: 16rpx; flex-wrap: wrap;
  .tag {
    padding: 12rpx 28rpx; border-radius: 8rpx; font-size: 26rpx; color: #666;
    background: #f7f8fa;
    &.active { color: #fff; background: #f23030; }
  }
}
.row-center {
  display: flex; justify-content: space-between; align-items: center; font-size: 28rpx;
}
</style>