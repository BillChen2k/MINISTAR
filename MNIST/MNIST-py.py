import numpy as np
import tensorflow as tf
import json

# MACOS PROBLEM
# Windows 下运行或许可以注释掉
import os
os.environ['KMP_DUPLICATE_LIB_OK']='True'

# Load raw data
with open("MNISTData/train-images-idx3-ubyte", "rb") as f:
    train_image = f.read()

with open("MNISTData/train-labels-idx1-ubyte", "rb") as f:
    train_label = f.read()

with open("MNISTData/t10k-images-idx3-ubyte", "rb") as f:
    test_image = f.read()

with open("MNISTData/t10k-labels-idx1-ubyte", "rb") as f:
    test_label = f.read()

# 读取图片数
trainCount = int.from_bytes(train_image[4:8], byteorder="big", signed=False) # 60000
testCount = int.from_bytes(test_image[4:8], byteorder="big", signed=False) # 10000
dimension = int.from_bytes(train_image[8:12], byteorder="big", signed=False)

firstP = int.from_bytes(train_label[8:9], byteorder="big", signed=False)

x_train = np.zeros((trainCount, dimension, dimension))
y_train = np.zeros((trainCount))
x_test = np.zeros((testCount, dimension, dimension))
y_test = np.zeros((testCount))

print("Starting to load data")

# Load train manually.

x_train = np.frombuffer(train_image[16: 16 + (dimension * dimension) * trainCount], dtype=np.ubyte).reshape((trainCount, dimension, dimension, 1))
y_train = np.frombuffer(train_label[8: 8 + trainCount], dtype=np.ubyte)
x_test = np.frombuffer(test_image[16: 16 + (dimension * dimension) * testCount], dtype=np.ubyte).reshape((testCount, dimension, dimension, 1))
y_test = np.frombuffer(test_label[8: 8 + trainCount], dtype=np.ubyte)

# 使用 Google 提供的 MINIST 数据集
# 需要下载且可能不能跨平台
mnist = tf.keras.datasets.mnist
(xx_train, yy_train), (xx_test, yy_test) = mnist.load_data()

# 归一化
x_train, x_test = x_train / 255.0, x_test / 255.0

print("Data loaded.")



BATCH_SIZE = 150


def np2list(array, digit=9):
    """
    Turn an numpy array into a Python list object.
    :param array:
    :param digit:
    :return:
    """
    if len(array.shape) == 1:
        return [round(one, digit) for one in array.tolist()]
    else:
        return [np2list(one) for one in array]
    # return np.round(array, decimals=digit).tolist()


# 自定义的模型，用于接管 train loop
# 在这个过程中可以得到当前的所有数据
# https://www.tensorflow.org/guide/keras/customizing_what_happens_in_fit
class ModelWithFeedback(tf.keras.Sequential):

    def train_step(self, data):
        # Unpack the data. Its structure depends on your model and
        # on what you pass to `fit()`.
        x, y = data
        with tf.GradientTape() as tape:
            y_pred = self(x, training=True)  # Forward pass
            # Compute the loss values
            # (the loss function is configured in `compile()`)
            loss = self.compiled_loss(y, y_pred, regularization_losses=self.losses)
        # Compute gradients
        trainable_vars = self.trainable_variables
        gradients = tape.gradient(loss, trainable_vars)
        # Update weights
        self.optimizer.apply_gradients(zip(gradients, trainable_vars))
        # Update metrics (includes the metric that tracks the loss)
        self.compiled_metrics.update_state(y, y_pred)
        # Return a dict mapping metric names to current value
        metricDict = {m.name: m.result() for m in self.metrics}

        return metricDict


# 原生模型，未使用
# model = tf.keras.models.Sequential([
#   tf.keras.layers.Flatten(input_shape=(28, 28)),
#   # tf.keras.layers.Dense(28, activation='relu'),
#   # tf.keras.layers.Dropout(0.2),
#   tf.keras.layers.Dense(10, activation = 'softmax')
# ])

model = ModelWithFeedback([
    tf.keras.layers.Conv2D(5, (5, 5), input_shape=(28, 28, 1)),
    tf.keras.layers.MaxPooling2D(pool_size=(3, 3)),
    tf.keras.layers.Flatten(input_shape=(8, 8)),
      # tf.keras.layers.Dense(28, activation='relu'),
      # tf.keras.layers.Dropout(0.2),
      tf.keras.layers.Dense(10, activation='softmax')
])

model.summary()

CURRENT_EPOCH = 0
modelParams = {
    "begin_epoch": 0,
    "begin_batch": 0,
    "end_epoch": 0,
    "end_batch": 0,
    "weights": []
}

SINGLEFILECOUNT = 0

# 回调类，在每一次迭代之后调用。备用。
class fitCallback(tf.keras.callbacks.Callback):



    def on_epoch_begin(self, epoch, logs=None):
        global CURRENT_EPOCH
        CURRENT_EPOCH = epoch
        return super().on_epoch_begin(epoch, logs)



    def on_batch_end(self, batch, logs):
        # 取得全连接层的权重。
        # 权重为二维列表，分别为 w 和 b。w 尺寸为 [784, 10], b 为 [10]，输入 x 为 [784, 1]，wx + b 即可得到最后的数值。
        # （前面的 Training Loop 里也有获取权重的方式）

        # 返回全连接层的权重，可以交给 Unity 处理

        layer_conv = self.model.layers[0]
        layer_maxpooling = self.model.layers[1]
        layer_flatten = self.model.layers[2]
        layer_dense = self.model.layers[3]

        # modelData = {
        #     "epoch": CURRENT_EPOCH,
        #     "batch": batch,
        #     "loss": round(logs["loss"], 4),
        #     "accuracy": round(logs["accuracy"], 4),
        #     "input": np2list(x_train[batch * BATCH_SIZE].reshape((28, 28))),
        #     "layers": [
        #         {
        #             "name": "conv",
        #             "w": np2list(layer_conv.get_weights()[0]),
        #             "b": np2list(layer_conv.get_weights()[1])
        #         },
        #         {
        #             "name": "dense",
        #             "w": np2list(layer_dense.get_weights()[0]),
        #             "b": np2list(layer_dense.get_weights()[1])
        #         },
        #     ]
        # }

        modelData = {
            "epoch": CURRENT_EPOCH,
            "batch": batch,
            "loss": round(logs["loss"], 4),
            "accuracy": round(logs["accuracy"], 4),
            "input": np2list(x_train[batch * BATCH_SIZE].reshape((28, 28))),
            "conv_layer": {
                "w": np2list(layer_conv.get_weights()[0]),
                "b": np2list(layer_conv.get_weights()[1])
            },
            "dense_layer": {
                "w": np2list(layer_dense.get_weights()[0]),
                "b": np2list(layer_dense.get_weights()[1])
            }
        }

        if len(modelParams["weights"]) == 0:
            modelParams["begin_batch"], modelParams["begin_epoch"] = batch, CURRENT_EPOCH

        modelParams["weights"].append(modelData)

        if len(modelParams["weights"]) >= 20:
            modelParams["end_batch"], modelParams["end_epoch"] = batch, CURRENT_EPOCH
            with open("ModelParamsNew/param_e{}b{}_e{}n{}.json".format(modelParams["begin_epoch"], modelParams["begin_batch"],
                                                                    modelParams["end_epoch"], modelParams["end_batch"]), "w+") as f:
                json.dump(modelParams, f, indent=1)
            modelParams["weights"] = []

        pass

model.compile(optimizer=tf.keras.optimizers.Adam(learning_rate=0.01),
              loss='sparse_categorical_crossentropy',
              metrics=['accuracy'],
              run_eagerly=True) # 必须要 run_eagerly

model.fit(x_train, y_train, epochs=3, callbacks = [fitCallback()], batch_size=BATCH_SIZE)

# model.load_weights("model_weights.h5")

print("Evaluating...")
model.evaluate(x_test,  y_test, verbose=2)



print("Saving weights...")

# with open("modelParams.json", "w+") as f:
#     json.dump(modelParams, f, indent=1)


pass