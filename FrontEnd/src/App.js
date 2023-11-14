import "./assets/css/App.css";
import Header from "./components/Header/Header";
import ItemChat from "./components/ItemChat/ItemChat";

function App() {
  return (
    <div className="App">
      <Header name={"Duong Nguyen"} time={"Truy cập 4 phút trước"} />
      <ItemChat name={"Duong Nguyen"} time={"4 giờ"} mess={"Hello world"} />
    </div>
  );
}

export default App;
